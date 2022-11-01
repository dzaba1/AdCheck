using Dzaba.AdCheck.ActiveDirectory.Contracts;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Linq;
using Dzaba.AdCheck.Utils;

namespace Dzaba.AdCheck.ActiveDirectory
{
    public static class Extensions
    {
        public static bool TryGetPropertyValue(this UserPrincipal principal, string name, out object[] result)
        {
            Require.NotNull(principal, nameof(principal));
            Require.NotWhiteSpace(name, nameof(name));

            var underlayingObject = principal.GetUnderlyingObject() as DirectoryEntry;
            if (underlayingObject != null)
            {
                return underlayingObject.TryGetPropertyValue(name, out result);
            }

            result = null;
            return false;
        }

        public static bool TryGetPropertyValue(this DirectoryEntry entry, string name, out object[] result)
        {
            Require.NotNull(entry, nameof(entry));
            Require.NotWhiteSpace(name, nameof(name));

            try
            {
                var values = entry.Properties[name];
                if (values != null)
                {
                    result = values.Cast<object>().ToArray();
                    return result.Any();
                }
            }
            catch (DirectoryServicesCOMException)
            {
                // swallow
            }

            result = null;
            return false;
        }

        public static T FirstValueOrDefault<T>(this DirectoryEntry entry, string name)
        {
            Require.NotNull(entry, nameof(entry));
            Require.NotWhiteSpace(name, nameof(name));

            if (entry.TryGetPropertyValue(name, out var values))
            {
                return (T)values.First();
            }

            return default;
        }

        public static long ToLong(this IADsLargeInteger largeInt)
        {
            Require.NotNull(largeInt, nameof(largeInt));

            long lowPart = largeInt.LowPart;
            long highPart = largeInt.HighPart;
            long num = lowPart | highPart << 32;
            return num;
        }
    }
}
