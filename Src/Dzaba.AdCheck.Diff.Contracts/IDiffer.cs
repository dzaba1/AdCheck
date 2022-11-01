using System;
using System.Collections.Generic;

namespace Dzaba.AdCheck.Diff.Contracts
{
    public interface IDiffer
    {
        IEnumerable<DiffReport> DiffWithNow(DateTime dateTime, IEnumerable<string> domains, DiffFiltering filtering);
        IEnumerable<DiffReport> DiffLatest(IEnumerable<string> domains, DiffFiltering filtering);
        IEnumerable<DiffReport> Diff(DateTime left, DateTime right, DiffFiltering filtering);
        IEnumerable<DiffReport> Diff(int leftPollingId, int rightPollingId, DiffFiltering filtering);
    }
}
