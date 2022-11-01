using Dzaba.AdCheck.DataAccess.Configuration;
using Dzaba.AdCheck.DataAccess.Contracts;
using Dzaba.AdCheck.DataAccess.Dal;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using Dzaba.AdCheck.DataAccess.Repository;

namespace Dzaba.AdCheck.DataAccess
{
    public static class Bootstrapper
    {
        public static void RegisterAdDatabase(this IServiceCollection services, Func<IServiceProvider, string> connectionString)
        {
            Require.NotNull(services, nameof(services));

            services.AddTransient<IModelConfiguration, AdUserConfiguration>();
            services.AddTransient<IModelConfiguration, UserRoleConfiguration>();

            services.AddDbContext<DatabaseContext>((c, b) =>
                {
                    var builder = b.UseLazyLoadingProxies();
                    var provider = c.GetRequiredService<ISqlServerProvider>();
                    provider.Configure(builder, connectionString(c));
                },
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddTransient<Func<DatabaseContext>>(c => () =>
            {
                var db = c.GetRequiredService<DatabaseContext>();
                db.Database.EnsureCreated();
                return db;
            });

            services.AddTransient<IRepositoryFactory, RepositoryFactory>();

            services.AddTransient<IPollingDal, PollingDal>();
            services.AddTransient<IUsersDal, UsersDal>();
            services.AddTransient<IRolesDal, RolesDal>();
        }
    }
}
