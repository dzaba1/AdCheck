using Dzaba.AdCheck.ActiveDirectory.Contracts;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Dzaba.AdCheck.ActiveDirectory
{
    public static class Bootstrapper
    {
        public static void RegisterAd(this IServiceCollection services)
        {
            Require.NotNull(services, nameof(services));

            services.AddTransient<IAdDal, AdDal>();
        }
    }
}
