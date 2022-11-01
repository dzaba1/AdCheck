using Dzaba.AdCheck.ActiveDirectory.Contracts;
using System.Reflection;
using Dzaba.AdCheck.Utils;

namespace Dzaba.AdCheck.ActiveDirectory
{
    internal sealed class AdMapDescriptor
    {
        private readonly AdMapAttribute mapAttribute;

        public AdMapDescriptor(AdMapAttribute mapAttribute, PropertyInfo property)
        {
            Require.NotNull(mapAttribute, nameof(mapAttribute));
            Require.NotNull(property, nameof(property));

            this.mapAttribute = mapAttribute;
            Property = property;
        }

        public PropertyInfo Property { get; }

        public string AdPropertyName => mapAttribute.PropertyName;

        public bool AutoSet => mapAttribute.AutoSet;
    }
}
