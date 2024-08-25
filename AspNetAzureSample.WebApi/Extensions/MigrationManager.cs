using AspNetAzureSample.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspNetAzureSample.Extensions
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var logger = webHost.Services.GetRequiredService<ILogger<Program>>();
                using var appContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                try
                {
                    appContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred during database migration.");
                    //Log errors or do anything you think it's needed
                    throw;
                }
            }

            return webHost;
        }
    }
}
