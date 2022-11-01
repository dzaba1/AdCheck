using Dzaba.AdCheck.Diff.Contracts;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Dzaba.AdCheck.Diff
{
    public static class Bootstrapper
    {
        public static void RegisterAdDiff(this IServiceCollection services)
        {
            Require.NotNull(services, nameof(services));

            services.AddTransient<IDiffer, Differ>();
        }
    }
}
