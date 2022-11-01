using System.Collections.Generic;

namespace Dzaba.AdCheck.ActiveDirectory.Contracts
{
    public interface IAdDal
    {
        IEnumerable<AdUser> GetAllUsers(string domain, bool parallel);
    }
}
