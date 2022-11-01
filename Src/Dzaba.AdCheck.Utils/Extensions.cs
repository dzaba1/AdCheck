using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dzaba.AdCheck.Utils
{
    public static class Extensions
    {
        public static bool HasCustomAttribute<T>(this MemberInfo memberInfo)
            where T : Attribute
        {
            Require.NotNull(memberInfo, nameof(memberInfo));

            return memberInfo.GetCustomAttribute<T>() != null;
        }

        public static IEnumerable<T> ForEachLazy<T>(this IEnumerable<T> source, Action<T> action)
        {
            Require.NotNull(source, nameof(source));
            Require.NotNull(action, nameof(action));

            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }
    }
}
