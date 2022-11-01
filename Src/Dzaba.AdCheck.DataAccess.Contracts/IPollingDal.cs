using System;
using System.Collections.Generic;

namespace Dzaba.AdCheck.DataAccess.Contracts
{
    public interface IPollingDal
    {
        int NewPolling();
        IPolling GetPolling(DateTime history);
        IPolling GetPolling(int id);
        IPolling GetLatest();
        IEnumerable<IPolling> GetAll();
    }
}
