using AspNetAzureSample.Configuration;
using AspNetAzureSample.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AspNetAzureSample.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string MaintenanceScope = "maintenance";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testConfigurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(
        [
            KeyValuePair.Create<string, string?>($"{Auth0Options.Name}:{nameof(Auth0Options.Enable)}", "true"),
            KeyValuePair.Create<string, string?>($"{Auth0Options.Name}:{nameof(Auth0Options.MaintenanceScopes)}", MaintenanceScope)
        ])
        .Build();

        builder.UseConfiguration(testConfigurationBuilder);
        builder.UseEnvironment(EnvironmentConfiguration.Testing);

        builder.ConfigureServices(services =>
        {
            // Remove existing Identity-related services and replace them with in-memory ones or mocks
            services.RemoveAll<DbContextOptions<ApplicationContext>>();
            services.RemoveAll<ApplicationContext>();

            // Crucially, remove all existing Authentication services/schemes
            // This prevents conflicts like "Scheme already exists: Identity.Application"
            services.RemoveAll<IAuthenticationService>();
            services.RemoveAll<IAuthenticationHandlerProvider>();
            services.RemoveAll<IAuthenticationSchemeProvider>();
            services.RemoveAll<IPostConfigureOptions<AuthenticationOptions>>();
            // Also remove Identity's own registration of UserManager, SignInManager to ensure clean slate
            services.RemoveAll<UserManager<IdentityUser>>();
            services.RemoveAll<SignInManager<IdentityUser>>();
            services.RemoveAll<IUserStore<IdentityUser>>();
            services.RemoveAll<IRoleStore<IdentityRole>>();

            // Add an in-memory database for testing Identity
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.DefaultScheme;
                options.DefaultChallengeScheme = TestAuthHandler.DefaultScheme;
                options.DefaultScheme = TestAuthHandler.DefaultScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.DefaultScheme, options => { });

            // Seed data for your in-memory database if needed for tests
            using var scope = services.BuildServiceProvider().CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationContext>();
            db.Database.EnsureCreated(); // Ensure the in-memory DB is created

            // You might want to seed test users here
            var userManager = scopedServices.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

            // Example: create a test user
            if (userManager.FindByNameAsync("testuser").Result == null)
            {
                var user = new IdentityUser { UserName = "testuser", Email = "test@example.com", EmailConfirmed = true };
                userManager.CreateAsync(user, "Test@123");
            }

            // Clear the database after each test if using a single factory instance
            // Or for each test, create a fresh WebApplicationFactory
            //db.SaveChanges();
        });
    }
}