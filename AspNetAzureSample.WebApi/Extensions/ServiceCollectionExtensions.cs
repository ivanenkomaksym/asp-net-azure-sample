using AspNetAzureSample.Authentication;
using AspNetAzureSample.Configuration;
using AspNetAzureSample.Models.Identity;
using AspNetAzureSample.Security;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace AspNetAzureSample.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static StorageOptions ConfigureStorage(this IServiceCollection services, ConfigurationManager configuration)
        {
            var storageOptions = new StorageOptions();
            configuration.Bind(StorageOptions.Name, storageOptions);

            if (storageOptions.StorageType == StorageOptions.StorageTypes.InMemory)
            {
                services.AddDbContext<ApplicationContext>(opts =>
                    opts.UseInMemoryDatabase("AppDb"));
            }
            else if (storageOptions.StorageType == StorageOptions.StorageTypes.MySql && storageOptions.MySqlConnection != null)
            {
                services.AddDbContext<ApplicationContext>(opts =>
                    opts.UseMySQL(storageOptions.MySqlConnection));
            }
            else if (storageOptions.StorageType == StorageOptions.StorageTypes.SqlServer)
            {
                services.AddDbContext<ApplicationContext>(opts =>
                    opts.UseSqlServer(storageOptions.SqlServerConnection));
            }

            return storageOptions;
        }

        public static void ConfigureAuthentication(this IServiceCollection services,
                                                   ConfigurationManager configuration,
                                                   ISwaggerConfigurator swaggerConfigurator)
        {
            var azureAdOptions = new AzureADOptions();
            configuration.Bind(AzureADOptions.Name, azureAdOptions);

            var auth0Options = new Auth0Options();
            configuration.Bind(Auth0Options.Name, auth0Options);

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var authenticationBuilder = services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = MultiSchemeAuthenticationExtensions.MultiAuthenticationScheme;
                    options.DefaultChallengeScheme = MultiSchemeAuthenticationExtensions.MultiAuthenticationScheme;
                })
                .AddCookie()
                .AddPolicyScheme(MultiSchemeAuthenticationExtensions.MultiAuthenticationScheme,
                                 MultiSchemeAuthenticationExtensions.MultiAuthenticationScheme,
                                 options =>
                                 {
                                     options.SelectDefaultSchemeForCurrentRequest();
                                 });
            var logger = loggerFactory.CreateLogger<CustomJwtBearerEvents>();

            AzureAuthenticationExtensions.ConfigureAuthentication(authenticationBuilder, configuration, logger);
            GoogleAuthenticationExtensions.ConfigureAuthentication(authenticationBuilder, configuration, logger);
            Auth0AuthenticationExtensions.ConfigureAuthentication(authenticationBuilder, configuration, logger);

            var shouldEnableOrganizationAccess = auth0Options.OrganizationId != null;

            services.AddAuthorization(opts =>
            {
                if (azureAdOptions.RoleName != null)
                    opts.AddPolicy(AuthorizationPolicies.ApplicationAccessPolicy, p => p.RequireClaim(ClaimConstants.Role, azureAdOptions.RoleName));

                if (auth0Options.MaintenanceScopes != null)
                    opts.AddPolicy(AuthorizationPolicies.CycleManagementPolicy, p => p.RequireClaim("scope", auth0Options.MaintenanceScopes));

                if (shouldEnableOrganizationAccess)
                    opts.AddPolicy(AuthorizationPolicies.OrganizationAccessPolicy, p => p.RequireClaim("org_id", auth0Options.OrganizationId));
            });

            if (shouldEnableOrganizationAccess)
                services.AddMvc(options => options.Filters.Add(new AuthorizeFilter(AuthorizationPolicies.OrganizationAccessPolicy)));

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(swaggerGenOptions =>
            {
                swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherForecast", Version = "v1" });
                swaggerGenOptions.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = swaggerConfigurator.BuildImplicitFlow()
                    }
                });

                swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "oauth2"
                                },
                            },
                            ["openid", "profile", "email"]
                        }
                    });
            });
        }
    }
}
