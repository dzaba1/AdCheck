namespace Dzaba.AdCheck.Polling.Contracts
{
    public sealed class PollOptions
    {
        public bool ParallelDomainsProcessing { get; set; }
        public bool ParallelAdUsersProcessing { get; set; }
    }
}
