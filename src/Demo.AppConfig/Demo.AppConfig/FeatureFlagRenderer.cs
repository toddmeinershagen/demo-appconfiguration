using Microsoft.FeatureManagement;
using System.Threading.Tasks;
using System;

namespace Demo.AppConfig
{
    partial class Program
    {
        public class FeatureFlagRenderer
        {
            private readonly IFeatureManager _manager;

            public FeatureFlagRenderer(IFeatureManager manager)
            {
                this._manager = manager;
            }

            public async Task RenderFlags(int clientId)
            {
                var appContext = new AppContext { ClientId = clientId };

                await Console.Out.WriteLineAsync($"\r\n{DateTime.Now}");
                await Console.Out.WriteLineAsync($"New Pay Grid:         {await _manager.IsEnabledAsync("NewPayGrid")}");
                await Console.Out.WriteLineAsync($"New Payroll Reports:  {await _manager.IsEnabledAsync("NewPayrollReports", appContext)}");
                await Console.Out.WriteLineAsync($"Use Minimum Wage:     {await _manager.IsEnabledAsync("UseMinimumWage", appContext)}");
            }
        }
    }
}
