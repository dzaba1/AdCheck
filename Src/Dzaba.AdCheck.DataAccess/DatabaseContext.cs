using Dzaba.AdCheck.DataAccess.Configuration;
using Dzaba.AdCheck.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Dzaba.AdCheck.DataAccess.Contracts.Model;

namespace Dzaba.AdCheck.DataAccess
{
    public class DatabaseContext : DbContext
    {
        private readonly IModelConfiguration[] modelConfigurations;

        public DatabaseContext(DbContextOptions<DatabaseContext> options,
            IEnumerable<IModelConfiguration> modelConfigurations)
            : base(options)
        {
            Require.NotNull(modelConfigurations, nameof(modelConfigurations));

            this.modelConfigurations = modelConfigurations.ToArray();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var configuration in modelConfigurations)
            {
                configuration.Configure(builder);
            }
        }

        public DbSet<AdUser> Users { get; set; }
        public DbSet<Polling> Pollings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
