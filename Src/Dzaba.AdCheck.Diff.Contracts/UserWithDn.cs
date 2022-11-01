using System.Collections.Generic;
using Dzaba.AdCheck.ActiveDirectory.Contracts;

namespace Dzaba.AdCheck.Diff.Contracts
{
    public class UserWithDn
    {
        public IAdUser User { get; set; }
        public IReadOnlyDictionary<string, DnToken[]> DnTokens { get; set; }
    }
}
