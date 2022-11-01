using System;

namespace Dzaba.AdCheck.DataAccess.Contracts
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IndexAttribute : Attribute
    {
        public bool Unique { get; set; }

        public string Name { get; set; }
    }
}
