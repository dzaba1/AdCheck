using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Dzaba.AdCheck.DataAccess.Contracts;
using Dzaba.AdCheck.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dzaba.AdCheck.DataAccess.Configuration
{
    internal abstract class EntityConfiguration<T> : IModelConfiguration
        where T : class
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<T>();

            SetIndexes(builder);
            SetDefaultValues(builder);

            Configure(builder);
        }

        private IEnumerable<PropertyInfo> GetMappedProperties()
        {
            return typeof(T)
                .GetProperties()
                .Where(p => !p.HasCustomAttribute<NotMappedAttribute>());
        }

        private void SetIndexes(EntityTypeBuilder<T> builder)
        {
            var indexes = GetMappedProperties()
                .Select(p => new { Index = p.GetCustomAttribute<IndexAttribute>(), PropertyName = p.Name })
                .Where(i => i.Index != null);

            foreach (var index in indexes)
            {
                var indexBuilder = builder.HasIndex(index.PropertyName)
                    .IsUnique(index.Index.Unique);
                if (!string.IsNullOrWhiteSpace(index.Index.Name))
                {
                    indexBuilder.HasName(index.Index.Name);
                }
            }
        }

        private void SetDefaultValues(EntityTypeBuilder<T> builder)
        {
            var defaultValues = GetMappedProperties()
                .Select(p => new { DefaultValue = p.GetCustomAttribute<DefaultValueAttribute>(), PropertyName = p.Name })
                .Where(i => i.DefaultValue != null);

            foreach (var defaultValue in defaultValues)
            {
                builder.Property(defaultValue.PropertyName)
                    .HasDefaultValue(defaultValue.DefaultValue.Value);
            }
        }

        protected abstract void Configure(EntityTypeBuilder<T> builder);
    }
}
