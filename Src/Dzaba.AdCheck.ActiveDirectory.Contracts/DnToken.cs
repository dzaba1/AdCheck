namespace Dzaba.AdCheck.ActiveDirectory.Contracts
{
    public class DnToken
    {
        public string Raw { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return Raw;
        }
    }
}
