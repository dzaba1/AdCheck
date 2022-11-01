using System.ComponentModel.DataAnnotations;

namespace Dzaba.AdCheck.Diff.Contracts
{
    public class SearchCondition
    {
        /// <summary>
        /// Property name.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// Operator.
        /// </summary>
        public SearchOperator Operator { get; set; }

        /// <summary>
        /// Property value. If operator is Any then this value is not used.
        /// </summary>
        public string Value { get; set; }
    }
}
