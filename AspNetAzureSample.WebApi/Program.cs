using AspNetAzureSample.Authentication;
using AspNetAzureSample.Authorization;
using AspNetAzureSample.Configuration;
using AspNetAzureSample.Extensions;
using AspNetAzureSample.Models.Identity;
using AspNetAzureSample.Security;
using AspNetAzureSample.UserProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddAzureWebAppDiagnostics();
builder.Host.UseNLog();

builder.Services.AddHealthChecks();

var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.ConfigureStorage(configuration);

// Conditionally add IdentityApiEndpoints ONLY if not in the "Testing" environment
if (!builder.Environment.IsEnvironment(EnvironmentConfiguration.Testing))
{
    services.AddIdentityApiEndpoints<IdentityUser>(opt =>
    {
        opt.Password.RequiredLength = 7;
        opt.Password.RequireDigit = false;
        opt.Password.RequireUppercase = false;
        opt.User.RequireUniqueEmail = true;
        opt.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();
}
else
{
    // In "Testing" environment, we will manually configure Identity (for UserManager/SignInManager)
    // and our custom test authentication scheme in CustomWebApplicationFactory
    services.AddIdentityCore<IdentityUser>() // Use AddIdentityCore for just the core Identity services
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddApiEndpoints() // Necessary for Identity features, but not for authentication schemes
            .AddDefaultTokenProviders();
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.MinimumAgePolicy, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new MinimumAgeRequirement("age", 18));
    });
});

builder.Services.AddTransient<IAuthorizationHandler, MinimumAgeHandler>();

builder.Services.AddCompositeAuthorizationResultTransformer(typeof(MinimumAgeAuthorizationResultTransformer),
                                                            typeof(UnsupportedOrganizationAuthorizationResultTransformer),
                                                            typeof(MaintenanceScopeAuthorizationResultTransformer));

services.AddTransient<IdentityDataSeeder>();
services.AddHostedService<SetupIdentityDataSeeder>();

services.AddAutoMapper(typeof(Program));

services.AddControllers();
services.AddSingleton<IUserProvider, DefaultUserProvider>();

var swaggerConfigurator = SwaggerConfiguration.BuildSwaggerConfigurator(configuration);

services.ConfigureAuthentication(configuration, swaggerConfigurator);

services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Read CORS configuration from appsettings.json
var corsOrigin = configuration.GetSection("UseCors:AllowOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            if (corsOrigin != null)
                builder.WithOrigins(corsOrigin)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
        });
});

var app = builder.Build();

app.ConfigureAppAuthentication(configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsEnvironment(EnvironmentConfiguration.Testing))
    app.UseMiddleware<AuthenticatedTestRequestMiddleware>();

app.UseRouting();
// UseCors must be placed after UseRouting and before UseAuthorization, see https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0
app.UseCors("AllowSpecificOrigin");

app.MapIdentityApi<IdentityUser>();

app.UseAuthentication();
// UseAuthorization must be placed after UseAuthentication, see https://stackoverflow.com/questions/65350040/signalr-issue-with-net-core-5-0-migration-app-usesignalr-app-useendpoints
app.UseAuthorization();

app.ConfigureSwagger(configuration, swaggerConfigurator);

app.MapControllers();
app.MapHealthChecks("/healthz").RequireAuthorization(AuthorizationPolicies.MinimumAgePolicy);

if (!app.Environment.IsEnvironment(EnvironmentConfiguration.Testing))
    app.MigrateDatabase(configuration);

app.Run();

public partial class Program { }