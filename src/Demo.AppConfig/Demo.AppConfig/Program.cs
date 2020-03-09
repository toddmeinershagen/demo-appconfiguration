using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System.Threading.Tasks;
using System;

namespace Demo.AppConfig
{
    partial class Program
    {
        public static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = InitializeConfiguration();
            IServiceCollection services = ConfigureServices(configuration);

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var clientId = Convert.ToInt32(configuration["clientId"]);
                var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();
                var renderer = new FeatureFlagRenderer(featureManager);

                while (true)
                {
                    await renderer.RenderFlags(clientId);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                };
            }
        }

        private static IConfigurationRoot InitializeConfiguration()
        {
            return new ConfigurationBuilder()
                //.AddJsonFile("appsettings.json", false, true)
                
                .AddAzureAppConfiguration(options =>
                {
                    options
                        .Connect(Environment.GetEnvironmentVariable("ConnectionString"))
                        .UseFeatureFlags(f => f.CacheExpirationTime = TimeSpan.FromSeconds(1));
                })
                
                .Build();
        }

        private static IServiceCollection ConfigureServices(IConfigurationRoot configuration)
        {
            IServiceCollection services = new ServiceCollection();

            services
                .AddSingleton<IConfiguration>(configuration)
                .AddFeatureManagement()
                .AddFeatureFilter<BetaFilter>();
            return services;
        }
    }
}
