using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Dzaba.AdCheck.DataAccess.SqlServer
{
    public static class Bootstrapper
    {
        public static void RegisterAdSqlServer(this IServiceCollection services)
        {
            Require.NotNull(services, nameof(services));

            services.AddTransient<ISqlServerProvider, SqlServerProvider>();
        }
    }
}
