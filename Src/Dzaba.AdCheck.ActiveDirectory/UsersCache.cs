using System;
using System.Collections.Concurrent;
using Dzaba.AdCheck.Utils;

namespace Dzaba.AdCheck.ActiveDirectory
{
    internal sealed class UsersCache
    {
        private readonly ConcurrentDictionary<string, string> dnCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string GetOrAddByDn(string value, Func<string, string> factory)
        {
            Require.NotWhiteSpace(value, nameof(value));
            Require.NotNull(factory, nameof(factory));

            return dnCache.GetOrAdd(value, factory);
        }
    }
}
