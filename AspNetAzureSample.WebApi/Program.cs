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

builder.Services.AddRazorPages();
builder.Services.AddHealthChecks();

var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.ConfigureStorage(configuration);

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.MinimumAgePolicy, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new MinimumAgeRequirement("age", 18));
    });
});

builder.Services.AddTransient<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, MinimumAgeAuthorizationResultTransformer>();

services.AddTransient<IdentityDataSeeder>();
services.AddHostedService<SetupIdentityDataSeeder>();

services.AddAutoMapper(typeof(Program));

services.AddControllers();
services.AddSingleton<IUserProvider, DefaultUserProvider>();

var swaggerConfigurator = SwaggerConfiguration.BuildSwaggerConfigurator(configuration);

services.ConfigureAuthentication(configuration, swaggerConfigurator);

services.AddControllersWithViews(options =>
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

app.ConfigureSwagger(configuration, swaggerConfigurator);

app.MapControllers();
app.MapHealthChecks("/healthz").RequireAuthorization(AuthorizationPolicies.MinimumAgePolicy);

app.MigrateDatabase(configuration);

app.Run();

public partial class Program { }