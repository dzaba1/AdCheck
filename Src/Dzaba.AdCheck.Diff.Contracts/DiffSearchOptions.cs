namespace Dzaba.AdCheck.Diff.Contracts
{
    public sealed class DiffSearchOptions
    {
        /// <summary>
        /// These are filters for getting users before making any diff.
        /// </summary>
        public SearchCondition[] UserConditions { get; set; }

        /// <summary>
        /// These are filters for checking differences.
        /// </summary>
        public SearchChangeCondition[] ChangeConditions { get; set; }

        /// <summary>
        /// You can force apllying 
        /// </summary>
        public SearchChangeOperator ChangeOperator { get; set; }
    }
}
