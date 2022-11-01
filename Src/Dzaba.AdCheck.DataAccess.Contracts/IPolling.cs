using System;

namespace Dzaba.AdCheck.DataAccess.Contracts
{
    public interface IPolling
    {
        int Id { get; set; }
        DateTime TimeStamp { get; set; }
    }
}
