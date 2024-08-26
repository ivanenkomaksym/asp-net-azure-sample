using AspNetAzureSample.Authentication;
using AspNetAzureSample.Configuration;
using AspNetAzureSample.Extensions;
using AspNetAzureSample.Models.Identity;
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

services.AddTransient<IdentityDataSeeder>();
services.AddHostedService<SetupIdentityDataSeeder>();

services.AddAutoMapper(typeof(Program));

services.AddControllers();
services.AddSingleton<IUserProvider, DefaultUserProvider>();

services.ConfigureAuthentication(configuration);

services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Read CORS configuration from appsettings.json
var corsOrigin = configuration["UseCors:AllowOrigin"];
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

app.ConfigureSwagger(configuration);

app.MapControllers();

app.MigrateDatabase(configuration);

app.Run();

public partial class Program { }