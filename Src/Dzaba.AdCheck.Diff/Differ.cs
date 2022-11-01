using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dzaba.AdCheck.ActiveDirectory;
using Dzaba.AdCheck.ActiveDirectory.Contracts;
using Dzaba.AdCheck.DataAccess.Contracts;
using Dzaba.AdCheck.Diff.Contracts;
using Dzaba.AdCheck.Polling.Contracts;
using Dzaba.AdCheck.Utils;

namespace Dzaba.AdCheck.Diff
{
    internal sealed class Differ : IDiffer
    {
        private static readonly IReadOnlyDictionary<string, PropertyInfo> UserProperties =
            typeof(IAdUser).GetProperties().ToDictionary(p => p.Name);

        private readonly IAdDal adDal;
        private readonly IUsersDal usersDal;
        private readonly IPollingDal pollingDal;
        private readonly PollOptions pollOptions;

        public Differ(IAdDal adDal,
            IUsersDal usersDal,
            IPollingDal pollingDal,
            PollOptions pollOptions)
        {
            Require.NotNull(adDal, nameof(adDal));
            Require.NotNull(usersDal, nameof(usersDal));
            Require.NotNull(pollingDal, nameof(pollingDal));
            Require.NotNull(pollOptions, nameof(pollOptions));

            this.adDal = adDal;
            this.usersDal = usersDal;
            this.pollingDal = pollingDal;
            this.pollOptions = pollOptions;
        }

        public IEnumerable<DiffReport> DiffWithNow(DateTime dateTime, IEnumerable<string> domains,
            DiffFiltering filtering)
        {
            Require.NotNull(domains, nameof(domains));

            var rightTimeStamp = DateTime.Now;
            var leftPolling = pollingDal.GetPolling(dateTime);

            var leftUsers = domains.SelectMany(d => adDal.GetAllUsers(d, pollOptions.ParallelAdUsersProcessing)).ToArray();
            var rightUsers = usersDal.GetFromPolling(leftPolling.Id).ToArray();

            return Diff(leftUsers, rightUsers, leftPolling.TimeStamp, rightTimeStamp, filtering);
        }

        public IEnumerable<DiffReport> DiffLatest(IEnumerable<string> domains, DiffFiltering filtering)
        {
            Require.NotNull(domains, nameof(domains));

            var rightTimeStamp = DateTime.Now;
            var leftPolling = pollingDal.GetLatest();

            var leftUsers = domains.SelectMany(d => adDal.GetAllUsers(d, pollOptions.ParallelAdUsersProcessing)).ToArray();
            var rightUsers = usersDal.GetFromPolling(leftPolling.Id).ToArray();

            return Diff(leftUsers, rightUsers, leftPolling.TimeStamp, rightTimeStamp, filtering);
        }

        public IEnumerable<DiffReport> Diff(DateTime left, DateTime right, DiffFiltering filtering)
        {
            var leftPolling = pollingDal.GetPolling(left);
            var rightPolling = pollingDal.GetPolling(right);

            var leftUsers = usersDal.GetFromPolling(leftPolling.Id).ToArray();
            var rightUsers = usersDal.GetFromPolling(rightPolling.Id).ToArray();

            return Diff(leftUsers, rightUsers, leftPolling.TimeStamp, rightPolling.TimeStamp, filtering);
        }

        public IEnumerable<DiffReport> Diff(int leftPollingId, int rightPollingId, DiffFiltering filtering)
        {
            var leftPolling = pollingDal.GetPolling(leftPollingId);
            var rightPolling = pollingDal.GetPolling(rightPollingId);

            var leftUsers = usersDal.GetFromPolling(leftPolling.Id).ToArray();
            var rightUsers = usersDal.GetFromPolling(rightPolling.Id).ToArray();

            return Diff(leftUsers, rightUsers, leftPolling.TimeStamp, rightPolling.TimeStamp, filtering);
        }

        private HashSet<string> GetDomainNames(IEnumerable<IAdUser> left, IEnumerable<IAdUser> right)
        {
            var query = left.Select(l => l.Domain)
                .Concat(right.Select(r => r.Domain));
            return new HashSet<string>(query, StringComparer.OrdinalIgnoreCase);
        }

        private IReadOnlyDictionary<string, IReadOnlyList<IAdUser>> GetUsersByDomain(IEnumerable<IAdUser> users, HashSet<string> domainNames)
        {
            var dict = new Dictionary<string, IReadOnlyList<IAdUser>>(StringComparer.OrdinalIgnoreCase);
            foreach (var domainName in domainNames)
            {
                dict.Add(domainName, new List<IAdUser>());
            }

            foreach (var user in users)
            {
                var list = (List<IAdUser>)dict[user.Domain];
                list.Add(user);
            }

            return dict;
        }

        private IEnumerable<DiffReport> Diff(IReadOnlyList<IAdUser> left, IReadOnlyList<IAdUser> right,
            DateTime leftTimeStamp, DateTime rightTimeStamp, DiffFiltering filtering)
        {
            var domainNames = GetDomainNames(left, right);

            var leftDomains = GetUsersByDomain(left, domainNames);
            var rightDomains = GetUsersByDomain(right, domainNames);

            foreach (var leftDomain in leftDomains)
            {
                var rightDomain = rightDomains[leftDomain.Key];

                var payload = new DiffPayload
                {
                    Domain = leftDomain.Key,
                    Left = leftDomain.Value,
                    Right = rightDomain,
                    LeftTimeStamp = leftTimeStamp,
                    RightTimeStamp = rightTimeStamp,
                    UserSearchProperties = filtering?.Search?.UserConditions,
                    DiffSearchProperties = filtering?.Search?.ChangeConditions,
                    ChangeOperator = filtering?.Search?.ChangeOperator ?? SearchChangeOperator.Any,
                    IgnoreAdded = filtering?.Selection?.IgnoreAdded == true,
                    IgnoreChanges = filtering?.Selection?.IgnoreChanges == true,
                    IgnoreDeleted = filtering?.Selection?.IgnoreDeleted == true,
                    PropertiesToCompare = GetPropertiesToCompare(filtering)
                };

                yield return DiffDomain(payload);
            }
        }

        private HashSet<string> GetPropertiesToCompare(DiffFiltering filtering)
        {
            HashSet<string> props = null;
            if (filtering?.DiffOptions?.PropertiesToCompare != null)
            {
                props =
                    new HashSet<string>(filtering.DiffOptions.PropertiesToCompare, StringComparer.OrdinalIgnoreCase);
            }

            return props;
        }

        private DiffReport DiffDomain(DiffPayload payload)
        {
            var leftDict = payload.Left.ToDictionary(u => u.Sid, StringComparer.OrdinalIgnoreCase);
            var rightDict = payload.Right.ToDictionary(u => u.Sid, StringComparer.OrdinalIgnoreCase);

            var deleted = new List<UserWithDn>();
            var added = new List<UserWithDn>();
            var changes = new Dictionary<string, UserChanges>();

            foreach (var leftUser in leftDict.Values)
            {
                var leftUserWithDn = new UserWithDn
                {
                    DnTokens = Dn.Parse(leftUser.Dn),
                    User = leftUser
                };

                if (rightDict.TryGetValue(leftUser.Sid, out var rightUser))
                {
                    if (!payload.IgnoreChanges)
                    {
                        var rightUserWithDn = new UserWithDn
                        {
                            DnTokens = Dn.Parse(rightUser.Dn),
                            User = rightUser
                        };

                        if (CanDiff(leftUserWithDn, rightUserWithDn, payload))
                        {
                            var userChanges = Diff(leftUserWithDn, rightUserWithDn, payload.PropertiesToCompare);
                            if (IsChangeOk(userChanges.Changes, payload.DiffSearchProperties))
                            {
                                changes.Add(leftUser.Sid, userChanges);
                            }
                        }
                    }
                }
                else
                {
                    if (!payload.IgnoreDeleted && CanBeAdded(leftUserWithDn, payload))
                    {
                        deleted.Add(leftUserWithDn);
                    }
                }
            }

            if (!payload.IgnoreAdded)
            {
                foreach (var rightUser in rightDict.Values)
                {
                    if (!leftDict.ContainsKey(rightUser.Sid))
                    {
                        var rightUserWithDn = new UserWithDn
                        {
                            DnTokens = Dn.Parse(rightUser.Dn),
                            User = rightUser
                        };

                        if (CanBeAdded(rightUserWithDn, payload))
                        {
                            added.Add(rightUserWithDn);
                        }
                    }
                }
            }

            return new DiffReport
            {
                Added = added.ToArray(),
                Changes = changes,
                Deleted = deleted.ToArray(),
                Domain = payload.Domain,
                LeftTimeStamp = payload.LeftTimeStamp,
                RightTimeStamp = payload.RightTimeStamp
            };
        }

        private IEnumerable<NameValuePair> EnumeratePropertyValues(UserWithDn user)
        {
            var userProp = UserProperties.Values
                .Select(p => new NameValuePair
                {
                    Name = p.Name,
                    Value = p.GetValue(user.User)?.ToString()
                });

            var tokens = user.DnTokens.Values
                .SelectMany(t => t)
                .Select(t => new NameValuePair
                {
                    Name = t.Name,
                    Value = t.Value
                });

            return userProp.Concat(tokens);
        }

        private bool AreValuesOkByAny(IEnumerable<string> values)
        {
            return !values.All(string.IsNullOrEmpty);
        }

        private bool IsUserOk(UserWithDn user, SearchCondition[] conditions)
        {
            if (conditions == null)
            {
                return true;
            }

            var propValues = EnumeratePropertyValues(user)
                .ToArray();

            foreach (var condition in conditions)
            {
                var values = propValues
                    .Where(p => string.Equals(condition.Name, p.Name, StringComparison.OrdinalIgnoreCase));

                if (condition.Operator == SearchOperator.Any)
                {
                    if (values.All(p => string.IsNullOrEmpty(p.Value)))
                    {
                        return false;
                    }
                }
                else if (condition.Operator == SearchOperator.Equal)
                {
                    var equalResult = false;
                    foreach (var value in values)
                    {
                        equalResult = string.Equals(value.Value, condition.Value, StringComparison.OrdinalIgnoreCase);
                        if (equalResult)
                        {
                            break;
                        }
                    }

                    if (!equalResult)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsChangeOk(IReadOnlyList<Change> changes, SearchChangeCondition[] conditions)
        {
            if (!changes.Any())
            {
                return false;
            }

            if (conditions == null)
            {
                return true;
            }

            foreach (var condition in conditions)
            {
                var values = changes.Where(c =>
                    string.Equals(c.Property, condition.Name, StringComparison.OrdinalIgnoreCase));

                if (condition.Operator == SearchOperator.Any)
                {
                    if (!values.Any())
                    {
                        return false;
                    }
                }
                else if (condition.Operator == SearchOperator.Equal)
                {
                    var equalResult = false;
                    foreach (var change in values)
                    {
                        var leftResult = string.Equals(change.OldValue, condition.Value,
                            StringComparison.OrdinalIgnoreCase);
                        var rightResult = string.Equals(change.NewValue, condition.Value,
                            StringComparison.OrdinalIgnoreCase);

                        switch (condition.ChangeOperator)
                        {
                            default:
                                equalResult = leftResult || rightResult;
                                break;
                            case SearchChangeOperator.OnlyLeft:
                                equalResult = leftResult;
                                break;
                            case SearchChangeOperator.OnlyRight:
                                equalResult = rightResult;
                                break;
                        }

                        if (equalResult)
                        {
                            break;
                        }
                    }

                    if (!equalResult)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CanDiff(UserWithDn left, UserWithDn right, DiffPayload payload)
        {
            switch (payload.ChangeOperator)
            {
                default: return CanBeAdded(left, payload) || CanBeAdded(right, payload);
                case SearchChangeOperator.OnlyLeft:
                    return CanBeAdded(left, payload);
                case SearchChangeOperator.OnlyRight:
                    return CanBeAdded(right, payload);
            }
        }

        private bool CanBeAdded(UserWithDn userWithDn, DiffPayload payload)
        {
            return IsUserOk(userWithDn, payload.UserSearchProperties);
        }

        private UserChanges Diff(UserWithDn leftUser, UserWithDn rightUser, HashSet<string> propertiesToCompare)
        {
            return new UserChanges
            {
                Left = leftUser,
                Right = rightUser,
                Changes = DiffProperties(leftUser.User, rightUser.User, propertiesToCompare).ToArray()
            };
        }

        private IEnumerable<Change> DiffProperties(IAdUser leftUser, IAdUser rightUser, HashSet<string> propertiesToCompare)
        {
            var toCompare = UserProperties.Values;
            if (propertiesToCompare != null)
            {
                toCompare = toCompare.Where(p => propertiesToCompare.Contains(p.Name));
            }

            foreach (var prop in toCompare)
            {
                var leftValue = prop.GetValue(leftUser);
                var rightValue = prop.GetValue(rightUser);

                if (!Equals(leftValue, rightValue))
                {
                    yield return new Change
                    {
                        Property = prop.Name,
                        NewValue = Convert.ToString(rightValue),
                        OldValue = Convert.ToString(leftValue)
                    };
                }
            }
        }
    }
}
