using System.Collections.Generic;

namespace Dzaba.AdCheck.Polling.Contracts
{
    public interface IPoller
    {
        void DownloadAll(IEnumerable<string> domains);
    }
}
