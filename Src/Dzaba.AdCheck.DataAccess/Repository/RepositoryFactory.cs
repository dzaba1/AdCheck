using Dzaba.AdCheck.Utils;
using System;

namespace Dzaba.AdCheck.DataAccess.Repository
{
    public interface IRepositoryFactory
    {
        IRepository<T> Create<T>() where T : class;
    }

    internal sealed class RepositoryFactory : IRepositoryFactory
    {
        private readonly Func<DatabaseContext> databaseContextFactory;

        public RepositoryFactory(Func<DatabaseContext> databaseContextFactory)
        {
            Require.NotNull(databaseContextFactory, nameof(databaseContextFactory));

            this.databaseContextFactory = databaseContextFactory;
        }

        public IRepository<T> Create<T>() where T : class
        {
            return new Repository<T>(databaseContextFactory(), true);
        }
    }
}
