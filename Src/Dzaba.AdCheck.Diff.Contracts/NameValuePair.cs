using System.ComponentModel.DataAnnotations;

namespace Dzaba.AdCheck.Diff.Contracts
{
    public class NameValuePair
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }
    }
}
