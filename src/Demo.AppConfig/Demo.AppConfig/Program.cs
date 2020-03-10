using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
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

                var timer = new System.Timers.Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
                timer.Elapsed += async (sender, e) =>
                {
                    await Refresher.RefreshAsync();
                };
                timer.Start();

                while (true)
                {
                    await renderer.RenderFlags(clientId);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                };
            }
        }

        private static IConfigurationRefresher Refresher;

        private static IConfigurationRoot InitializeConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                
                .AddAzureAppConfiguration(options =>
                {
                    options
                        .Connect(Environment.GetEnvironmentVariable("ConnectionString"))
                        .ConfigureRefresh(r => r.SetCacheExpiration(TimeSpan.FromSeconds(5)))
                        .UseFeatureFlags(f => f.CacheExpirationTime = TimeSpan.FromSeconds(1));

                    Refresher = options.GetRefresher();
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
