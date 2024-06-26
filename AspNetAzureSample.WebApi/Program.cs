using AspNetAzureSample.Authentication;
using AspNetAzureSample.Configuration;
using AspNetAzureSample.Extensions;
using AspNetAzureSample.Models.Identity;
using AspNetAzureSample.Security;
using AspNetAzureSample.UserProviders;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddAzureWebAppDiagnostics();
builder.Host.UseNLog();

builder.Services.AddRazorPages();

var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
var azureadOptions = new AzureADOptions();
configuration.Bind(AzureADOptions.Name, azureadOptions);

var googleOptions = new GoogleOptions();
configuration.Bind(GoogleOptions.Name, googleOptions);

var swaggerOptions = new SwaggerOptions();
configuration.Bind(SwaggerOptions.Name, swaggerOptions);

var storageOptions = new StorageOptions();
configuration.Bind(StorageOptions.Name, storageOptions);

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddDebug();
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

if (storageOptions.StorageType == StorageOptions.StorageTypes.InMemory)
{
    services.AddDbContext<ApplicationContext>(opts =>
        opts.UseInMemoryDatabase("AppDb"));
}
else if (storageOptions.StorageType == StorageOptions.StorageTypes.MySql)
{
    services.AddDbContext<ApplicationContext>(opts =>
        opts.UseMySQL(storageOptions.MySqlConnection));
}
else if (storageOptions.StorageType == StorageOptions.StorageTypes.SqlServer)
{
    services.AddDbContext<ApplicationContext>(opts =>
        opts.UseSqlServer(storageOptions.SqlServerConnection));
}

services.AddIdentityApiEndpoints<IdentityUser>(opt =>
{
    opt.Password.RequiredLength = 7;
    opt.Password.RequireDigit = false;
    opt.Password.RequireUppercase = false;

    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()   // AddIdentityApiEndpoints doesn't configure roles by default unline AddIdentity, so add it manually
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

services.AddTransient<IdentityDataSeeder>();
services.AddHostedService<SetupIdentityDataSeeder>();

services.AddAutoMapper(typeof(Program));

services.AddControllers();
services.AddSingleton<IUserProvider, DefaultUserProvider>();

if (azureadOptions.Enable && googleOptions.Enable)
{
    services.AddAuthentication(options =>
        {
            options.DefaultScheme = MultiSchemeAuthenticationExtensions.AzureOrGoogleAuthScheme;
            options.DefaultChallengeScheme = MultiSchemeAuthenticationExtensions.AzureOrGoogleAuthScheme;
        })
        .AddCookie()
        .AddJwtBearer("Google", options =>
        {
            options.UseGoogle(clientId: googleOptions.ClientId ?? string.Empty);
            options.Events = new CustomJwtBearerEvents(loggerFactory.CreateLogger<CustomJwtBearerEvents>());
        })
        .AddPolicyScheme(MultiSchemeAuthenticationExtensions.AzureOrGoogleAuthScheme,
                         MultiSchemeAuthenticationExtensions.AzureOrGoogleAuthScheme,
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

// Read CORS configuration from appsettings.json
var corsOrigin = configuration["UseCors:AllowOrigin"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins(corsOrigin)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

if (app.Environment.IsEnvironment(EnvironmentConfiguration.Testing))
    app.UseMiddleware<AuthenticatedTestRequestMiddleware>();

app.UseRouting();
// UseCors must be placed after UseRouting and before UseAuthorization, see https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0
app.UseCors("AllowSpecificOrigin");

app.MapIdentityApi<IdentityUser>();

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
    if (azureadOptions.ClientID != null)
        c.OAuthConfigObject.AdditionalQueryStringParams = new Dictionary<string, string> { { "resource", azureadOptions.ClientID } };
});

app.MapControllers();

if (storageOptions.StorageType != StorageOptions.StorageTypes.InMemory)
    app.MigrateDatabase();

app.Run();

public partial class Program { }