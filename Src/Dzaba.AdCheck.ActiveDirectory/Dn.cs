using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dzaba.AdCheck.ActiveDirectory.Contracts;
using Dzaba.AdCheck.Utils;

namespace Dzaba.AdCheck.ActiveDirectory
{
    public static class Dn
    {
        private static readonly Regex DnRegex = new Regex(@"(?<Token>(?<Name>[A-Z]{2})=(?<Value>[^,]+))");

        public static IReadOnlyDictionary<string, DnToken[]> Parse(string dn)
        {
            Require.NotWhiteSpace(dn, nameof(dn));

            var comparer = StringComparer.OrdinalIgnoreCase;

            return ParseTokens(dn)
                .GroupBy(t => t.Name, comparer)
                .ToDictionary(g => g.Key, g => g.ToArray(), comparer);
        }

        public static IEnumerable<DnToken> ParseTokens(string dn)
        {
            Require.NotWhiteSpace(dn, nameof(dn));

            var matches = DnRegex.Matches(dn);
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                yield return new DnToken
                {
                    Raw = match.Value,
                    Name = match.Groups["Name"].Value,
                    Value = match.Groups["Value"].Value,
                    Index = i
                };
            }
        }
    }
}
