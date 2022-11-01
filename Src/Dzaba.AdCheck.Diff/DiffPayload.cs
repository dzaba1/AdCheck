using Dzaba.AdCheck.ActiveDirectory.Contracts;
using System;
using System.Collections.Generic;
using Dzaba.AdCheck.Diff.Contracts;

namespace Dzaba.AdCheck.Diff
{
    internal sealed class DiffPayload
    {
        public string Domain { get; set; }
        public IEnumerable<IAdUser> Left { get; set; }
        public IEnumerable<IAdUser> Right { get; set; }
        public DateTime LeftTimeStamp { get; set; }
        public DateTime RightTimeStamp { get; set; }
        public HashSet<string> PropertiesToCompare { get; set; }
        public SearchCondition[] UserSearchProperties { get; set; }
        public SearchChangeCondition[] DiffSearchProperties { get; set; }
        public SearchChangeOperator ChangeOperator { get; set; }
        public bool IgnoreAdded { get; set; }
        public bool IgnoreDeleted { get; set; }
        public bool IgnoreChanges { get; set; }
    }
}
