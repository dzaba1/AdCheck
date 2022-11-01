using System.Collections.Generic;
using Dzaba.AdCheck.ActiveDirectory.Contracts;

namespace Dzaba.AdCheck.DataAccess.Contracts
{
    public interface IUsersDal
    {
        void SaveUsers(IEnumerable<AdUser> users, int pollingId);
        IEnumerable<IAdUser> GetFromPolling(int pollingId);
    }
}
