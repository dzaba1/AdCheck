namespace Dzaba.AdCheck.Diff.Contracts
{
    public sealed class DiffSelectionOptions
    {
        public bool? IgnoreAdded { get; set; }
        public bool? IgnoreDeleted { get; set; }
        public bool? IgnoreChanges { get; set; }
    }
}
