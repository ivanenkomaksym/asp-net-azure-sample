namespace AspNetAzureSample.Models.Identity
{
    public class SetupIdentityDataSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public SetupIdentityDataSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IdentityDataSeeder>();

                await seeder.SeedAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
