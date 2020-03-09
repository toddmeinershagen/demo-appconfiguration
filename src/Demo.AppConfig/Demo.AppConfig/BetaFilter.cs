using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System.Threading.Tasks;

namespace Demo.AppConfig
{
    partial class Program
    {
        public class BetaFilter :
            IContextualFeatureFilter<AppContext>
        {
            public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext featureFilterContext, AppContext appContext)
            {
                var settings = featureFilterContext.Parameters.Get<BetaFilterSettings>();
                
                //var settings = new { ClientIds = new List<int>() };
                //featureFilterContext.Parameters.Bind(settings);
                
                return Task.FromResult(settings.ClientIds.Contains(appContext.ClientId));
            }
        }
    }
}
