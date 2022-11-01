using System;

namespace Dzaba.AdCheck.Utils
{
    public static class Require
    {
        public static void NotNull(object obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void NotWhiteSpace(string str, string paramName)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(paramName, $"The parameter '{paramName}' can't be null, empty or white space.");
            }
        }
    }
}
