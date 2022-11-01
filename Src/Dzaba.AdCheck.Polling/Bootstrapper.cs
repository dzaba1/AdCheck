using Dzaba.AdCheck.Polling.Contracts;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Dzaba.AdCheck.Polling
{
    public static class Bootstrapper
    {
        public static void RegisterAdPolling(this IServiceCollection services)
        {
            Require.NotNull(services, nameof(services));

            services.AddTransient<IPoller, Poller>();
        }
    }
}
