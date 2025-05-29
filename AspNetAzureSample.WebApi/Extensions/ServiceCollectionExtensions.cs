using AspNetAzureSample.Authentication;
using AspNetAzureSample.Configuration;
using AspNetAzureSample.Models.Identity;
using AspNetAzureSample.Security;
using AspNetAzureSample.Validation;
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

        public static void ConfigureAuthentication(this IServiceCollection services, ConfigurationManager configuration)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var azureadOptions = new AzureADOptions();
            configuration.Bind(AzureADOptions.Name, azureadOptions);

            var googleOptions = new GoogleOptions();
            configuration.Bind(GoogleOptions.Name, googleOptions);

            var auth0Options = new Auth0Options();
            configuration.Bind(Auth0Options.Name, auth0Options);

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

            services.AddAuthorization(opts =>
            {
                if (azureadOptions.RoleName != null)
                    opts.AddPolicy(AuthorizationPolicies.ApplicationAccessPolicy, p => p.RequireClaim(ClaimConstants.Role, azureadOptions.RoleName));

                opts.AddPolicy(AuthorizationPolicies.CycleManagementPolicy, p => p.RequireClaim("permissions", auth0Options.CycleManagementPermission));
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherForecast", Version = "v1" });

                if (auth0Options.Enable)
                {
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{auth0Options.Authority}authorize"),
                                TokenUrl = new Uri($"{auth0Options.Authority}auth2/token"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "audience", auth0Options.Audience }
                                }
                            }
                        }
                    });
                }
                else if (azureadOptions.Enable)
                {
                    var host = azureadOptions.Instance;
                    var tenantId = azureadOptions.TenantId;
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Implicit = new OpenApiOAuthFlow()
                            {
                                AuthorizationUrl = new Uri($"{host}/{tenantId}/oauth2/authorize"),
                                TokenUrl = new Uri($"{host}/{tenantId}/oauth2/v2.0/token")
                            }
                        }
                    });
                }

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "oauth2"
                                },
                            },
                            new List <string> { auth0Options.Audience }
                        }
                    });
            });
        }
    }
}
