using AspNetAzureSample.Authentication;
using AspNetAzureSample.Configuration;
using AspNetAzureSample.Models.Identity;
using AspNetAzureSample.Security;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
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

            if (azureadOptions.Enable && googleOptions.Enable)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = MultiSchemeAuthenticationExtensions.AzureOrGoogleOrAuth0AuthScheme;
                    options.DefaultChallengeScheme = MultiSchemeAuthenticationExtensions.AzureOrGoogleOrAuth0AuthScheme;
                })
                    .AddCookie()
                    .AddJwtBearer(MultiSchemeAuthenticationExtensions.GoogleScheme, options =>
                    {
                        options.UseGoogle(clientId: googleOptions.ClientId ?? string.Empty);
                        options.Events = new CustomJwtBearerEvents(loggerFactory.CreateLogger<CustomJwtBearerEvents>());
                    })
                    .AddJwtBearer(MultiSchemeAuthenticationExtensions.Auth0Scheme, options =>
                    {
                        options.Authority = auth0Options.Authority;
                        options.Audience = auth0Options.Audience;
                    })
                    .AddPolicyScheme(MultiSchemeAuthenticationExtensions.AzureOrGoogleOrAuth0AuthScheme,
                                     MultiSchemeAuthenticationExtensions.AzureOrGoogleOrAuth0AuthScheme,
                                     options =>
                                     {
                                         options.SelectDefaultSchemeForCurrentRequest();
                                     })
                    .AddMicrosoftIdentityWebApi(jwtOptions =>
                    {
                        configuration.GetSection(AzureADOptions.Name).Bind(jwtOptions);
                        jwtOptions.TokenValidationParameters.IssuerValidator = (string issuer,
                                                                                SecurityToken securityToken,
                                                                                TokenValidationParameters validationParameters) =>
                        {
                            return CustomIssuerValidator.ValidateSpecificIssuers(issuer, securityToken, validationParameters, azureadOptions.AcceptedTenantIds);
                        };
                        jwtOptions.Events = new CustomJwtBearerEvents(loggerFactory.CreateLogger<CustomJwtBearerEvents>());
                    },
                    msIdentityOptions =>
                    {
                        configuration.GetSection(AzureADOptions.Name).Bind(msIdentityOptions);
                    });

                // Authorization
                services.AddAuthorization(options =>
                {
                    // #TODO: Doesn't seem to be really needed
                    //var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    //    JwtBearerDefaults.AuthenticationScheme,
                    //    "Google",
                    //    "Identity.BearerAndApplication");
                    //defaultAuthorizationPolicyBuilder =
                    //    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                    //options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
                });
            }
            else if (azureadOptions.Enable)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddMicrosoftIdentityWebApi(jwtOptions =>
                        {
                            configuration.GetSection(AzureADOptions.Name).Bind(jwtOptions);
                            jwtOptions.TokenValidationParameters.IssuerValidator = (string issuer,
                                                                                    SecurityToken securityToken,
                                                                                    TokenValidationParameters validationParameters) =>
                            {
                                return CustomIssuerValidator.ValidateSpecificIssuers(issuer, securityToken, validationParameters, azureadOptions.AcceptedTenantIds);
                            };
                            jwtOptions.Events = new CustomJwtBearerEvents(loggerFactory.CreateLogger<CustomJwtBearerEvents>());
                        },
                        msIdentityOptions =>
                        {
                            configuration.GetSection(AzureADOptions.Name).Bind(msIdentityOptions);
                        });
            }
            else if (googleOptions.Enable)
            {
                // Works - cookie-based authn
                //services.AddAuthentication(options =>
                //{
                //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                //}).AddCookie()
                //.AddGoogle(options =>
                //{
                //    options.ClientId = configuration["Google:ClientId"] ?? string.Empty;
                //    options.ClientSecret = configuration["Google:ClientSecret"] ?? string.Empty;
                //});
                //services.AddSingleton<IUserProvider, DefaultUserProvider>();

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(options =>
                {
                    options.UseGoogle(clientId: googleOptions.ClientId ?? string.Empty);
                    options.Events = new CustomJwtBearerEvents(loggerFactory.CreateLogger<CustomJwtBearerEvents>());
                });
            }

            services.AddAuthorization(opts =>
            {
                if (azureadOptions.RoleName != null)
                    opts.AddPolicy(AuthorizationPolicies.ApplicationAccessPolicy, p => p.RequireClaim(ClaimConstants.Role, azureadOptions.RoleName));
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherForecast", Version = "v1" });

                if (azureadOptions.Enable)
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

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "oauth2"
                                },
                            },
                            new List <string> {}
                        }
                    });
                }
            });
        }
    }
}
