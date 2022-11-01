using System;
using System.Collections.Generic;

namespace Dzaba.AdCheck.Diff.Contracts
{
    public sealed class DiffReport
    {
        public string Domain { get; set; }
        public DateTime LeftTimeStamp { get; set; }
        public DateTime RightTimeStamp { get; set; }

        public UserWithDn[] Added { get; set; }
        public UserWithDn[] Deleted { get; set; }

        public IReadOnlyDictionary<string, UserChanges> Changes { get; set; }
    }
}
