namespace Dzaba.AdCheck.Diff.Contracts
{
    public sealed class UserChanges
    {
        public UserWithDn Left { get; set; }
        public UserWithDn Right { get; set; }

        public Change[] Changes { get; set; }
    }
}
