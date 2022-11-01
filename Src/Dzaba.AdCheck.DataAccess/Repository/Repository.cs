using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dzaba.AdCheck.Utils;

namespace Dzaba.AdCheck.DataAccess.Repository
{
    public interface IRepository<T> : IDisposable
        where T : class
    {
        IQueryable<T> Get();
        IQueryable<TOther> GetOther<TOther>() where TOther : class;
        void Insert(T entity);
        int SaveChanges();
        void AddRange(IEnumerable<T> entities);
    }

    internal sealed class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly DatabaseContext databaseContext;
        private readonly bool disposeContext;

        public Repository(DatabaseContext databaseContext, bool disposeContext)
        {
            Require.NotNull(databaseContext, nameof(databaseContext));

            this.databaseContext = databaseContext;
            this.disposeContext = disposeContext;
        }

        public void Dispose()
        {
            if (disposeContext && databaseContext != null)
            {
                databaseContext.Dispose();
            }
        }

        public IQueryable<T> Get()
        {
            return GetOther<T>();
        }

        public IQueryable<TOther> GetOther<TOther>() where TOther : class
        {
            return databaseContext.Set<TOther>();
        }

        public void Insert(T entity)
        {
            Require.NotNull(entity, nameof(entity));

            Validate(entity);
            
            var set = databaseContext.Set<T>();
            set.Add(entity);
        }

        private void Validate(T entity)
        {
            Validator.ValidateObject(entity, new ValidationContext(entity), true);
        }

        public int SaveChanges()
        {
            return databaseContext.SaveChanges();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Require.NotNull(entities, nameof(entities));

            var set = databaseContext.Set<T>();
            var temp = entities
                .ForEachLazy(Validate);
            set.AddRange(temp);
        }
    }
}
