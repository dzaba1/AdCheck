using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using Dzaba.AdCheck.ActiveDirectory.Contracts;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.Logging;

namespace Dzaba.AdCheck.ActiveDirectory
{
    internal sealed class AdDal : IAdDal
    {
        private readonly ILogger<AdDal> logger;

        public AdDal(ILogger<AdDal> logger)
        {
            Require.NotNull(logger, nameof(logger));

            this.logger = logger;
        }

        public IEnumerable<AdUser> GetAllUsers(string domain, bool parallel)
        {
            Require.NotWhiteSpace(domain, nameof(domain));

            var descriptors = GetMapDescriptors<AdUser>().ToArray();

            using (var dirEntry = new DirectoryEntry($"LDAP://{domain}/{FormatDomain(domain)}"))
            {
                using (var searcher = new DirectorySearcher(dirEntry, "(&(objectClass=user)(objectCategory=person))",
                    GetPropertiesToLoad(descriptors).ToArray()))
                {
                    searcher.PageSize = 1000;

                    logger.LogInformation("Start getting all users from domain '{Domain}'", domain);
                    var searchPerfWatch = Stopwatch.StartNew();
                    using (var adResult = searcher.FindAll())
                    {
                        searchPerfWatch.Stop();
                        logger.LogInformation(
                            "Searching all users from domain '{Domain}' took {Elapsed}. Transforming now.", domain,
                            searchPerfWatch.Elapsed);

                        var cache = new UsersCache();
                        var searchResult = adResult
                            .Cast<SearchResult>()
                            .Select(s => s.GetDirectoryEntry())
                            .Where(e => e != null);

                        var results = searchResult
                            .Select(u => Transform(u, domain, descriptors, cache, dirEntry));

                        if (parallel)
                        {
                            var result = new ConcurrentBag<AdUser>();
                            logger.LogDebug("Parallel users processing is enabled.");
                            Parallel.ForEach(results, adUser => { result.Add(adUser); });
                            return result;
                        }

                        return results.ToArray();
                    }
                }
            }
        }

        private IEnumerable<string> GetPropertiesToLoad(IEnumerable<AdMapDescriptor> descriptors)
        {
            return descriptors.Select(d => d.AdPropertyName)
                .Distinct();
        }

        private IEnumerable<AdMapDescriptor> GetMapDescriptors<T>()
        {
            return typeof(T)
                .GetProperties()
                .Select(p => new { Attr = p.GetCustomAttribute<AdMapAttribute>(), Property = p })
                .Where(p => p.Attr != null)
                .Select(p => new AdMapDescriptor(p.Attr, p.Property));
        }

        private AdUser Transform(DirectoryEntry directoryEntry, string domain, IEnumerable<AdMapDescriptor> descriptors,
            UsersCache cache, DirectoryEntry root)
        {
            var perfWatch = Stopwatch.StartNew();

            var result = new AdUser
            {
                Domain = domain
            };

            foreach (var descriptor in descriptors.Where(d => d.AutoSet))
            {
                if (directoryEntry.TryGetPropertyValue(descriptor.AdPropertyName, out var values))
                {
                    object valueToSet = descriptor.Property.PropertyType == typeof(DateTime?)
                        ? GetDateTime(directoryEntry, descriptor.AdPropertyName)
                        : values.First();

                    descriptor.Property.SetValue(result, valueToSet);
                }
            }

            var sidRaw = directoryEntry.FirstValueOrDefault<byte[]>("objectSid");
            if (sidRaw != null)
            {
                var sec = new SecurityIdentifier(sidRaw, 0);
                result.Sid = sec.Value;
            }

            var guidRaw = directoryEntry.FirstValueOrDefault<byte[]>("objectGUID");
            if (guidRaw != null)
            {
                result.Guid = new Guid(guidRaw);
            }

            var uac = (UserFlags)directoryEntry.FirstValueOrDefault<int>("userAccountControl");
            result.Disabled = uac.HasFlag(UserFlags.AccountDisabled);
            result.PasswordNeverExpires = uac.HasFlag(UserFlags.PasswordDoesNotExpire);
            result.PasswordNotRequired = uac.HasFlag(UserFlags.PasswordNotRequired);
            result.UserCannotChangePassword = uac.HasFlag(UserFlags.PasswordCannotChange);
            result.DelegationPermitted = !uac.HasFlag(UserFlags.AccountNotDelegated);

            try
            {
                result.Manager = GetSamAccountNameByDn(result.ManagerDn, cache, root);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error getting manager of {Domain}/{Account}", domain, result.AccountName);
            }

            perfWatch.Stop();
            logger.LogDebug("Transforming {Domain}/{Account} took {Elapsed}", domain, result.AccountName,
                perfWatch.Elapsed);
            return result;
        }

        private string GetSamAccountNameByDn(string dn, UsersCache cache, DirectoryEntry root)
        {
            if (string.IsNullOrWhiteSpace(dn))
            {
                return null;
            }

            return cache.GetOrAddByDn(dn, d =>
            {
                var filter = $"(&(objectClass=user)(objectCategory=person)(distinguishedName={dn}))";
                var propsToLoad = new[]
                {
                    "objectClass", "objectCategory", "distinguishedName", "sAMAccountName"
                };

                using (var searcher = new DirectorySearcher(root, filter, propsToLoad))
                {
                    var result = searcher.FindOne();
                    if (result != null)
                    {
                        return result.GetDirectoryEntry().FirstValueOrDefault<string>("sAMAccountName");
                    }

                    return null;
                }
            });
        }

        private string FormatDomain(string domain)
        {
            var format = domain.Split('.')
                .Select(s => $"DC={s}");
            return string.Join(",", format);
        }

        private DateTime? GetDateTime(DirectoryEntry directoryEntry, string name)
        {
            if (!directoryEntry.TryGetPropertyValue(name, out var values))
            {
                return null;
            }

            var value = values.First();
            long num = 0;
            if (value is long l)
            {
                num = l;
            }
            else if (value is IADsLargeInteger li)
            {
                num = li.ToLong();
            }

            if (num > 0 && num < 9223372036854775807)
            {
                try
                {
                    return DateTime.FromFileTimeUtc(num);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
