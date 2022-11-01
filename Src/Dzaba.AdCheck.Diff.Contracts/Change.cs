namespace Dzaba.AdCheck.Diff.Contracts
{
    public sealed class Change
    {
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
