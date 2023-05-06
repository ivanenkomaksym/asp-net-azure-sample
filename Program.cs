using AspNetAzureSample.Configuration;
using AspNetAzureSample.Security;
using AspNetAzureSample.UserProviders;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
var azureadOptions = new AzureADOptions();
configuration.Bind(AzureADOptions.Name, azureadOptions);

var swaggerOptions = new SwaggerOptions();
configuration.Bind(SwaggerOptions.Name, swaggerOptions);

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddDebug();
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddControllers();

if (azureadOptions.Enable)
{
    services.AddSingleton<IUserProvider, DefaultUserProvider>();

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
else
{
    services.AddSingleton<IUserProvider, BearerTokenUserProvider>();

    services.AddAuthentication(FixedAuthenticationHandler.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, FixedAuthenticationHandler>(FixedAuthenticationHandler.AuthenticationScheme, null);
}

services.AddAuthorization(opts =>
{
    opts.AddPolicy(AuthorizationPolicies.ApplicationAccessPolicy, p => p.RequireClaim(ClaimConstants.Role, azureadOptions.RoleName));
});

services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
// UseCors must be placed after UseRouting and before UseAuthorization, see https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0
app.UseCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
});

app.UseAuthentication();
// UseAuthorization must be placed after UseAuthentication, see https://stackoverflow.com/questions/65350040/signalr-issue-with-net-core-5-0-migration-app-usesignalr-app-useendpoints
app.UseAuthorization();

app.MapRazorPages();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecast v1");
    c.OAuthClientId(swaggerOptions.ClientID);
    c.OAuthClientSecret(swaggerOptions.ClientSecret);
    c.OAuthRealm(azureadOptions.ClientID);
    c.OAuthScopeSeparator(" ");
    // Needed for AuthorizationUrl = new Uri($"{host}/{tenantId}/oauth2/authorize")
    c.OAuthConfigObject.AdditionalQueryStringParams = new Dictionary<string, string> { { "resource", azureadOptions.ClientID } };
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
