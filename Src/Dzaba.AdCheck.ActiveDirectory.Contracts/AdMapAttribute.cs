using System;

namespace Dzaba.AdCheck.ActiveDirectory.Contracts
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AdMapAttribute : Attribute
    {
        public AdMapAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public string UserPrincipalPropertyName { get; set; }

        public bool AutoSet { get; set; } = true;
    }
}
