namespace Dzaba.AdCheck.Diff.Contracts
{
    /// <summary>
    /// Operators for filtering users.
    /// </summary>
    public enum SearchOperator
    {
        /// <summary>
        /// Any means that some user property has to have some value assigned.
        /// </summary>
        Any,

        /// <summary>
        /// Equal means that some user property has to have some value and it should be equal to the Value provided in the request.
        /// </summary>
        Equal
    }
}
