using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dzaba.AdCheck.ActiveDirectory.Contracts;
using Dzaba.AdCheck.DataAccess.Contracts;
using Dzaba.AdCheck.DataAccess.Repository;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.Logging;

namespace Dzaba.AdCheck.DataAccess.Dal
{
    internal sealed class UsersDal : IUsersDal
    {
        private readonly ILogger<UsersDal> logger;
        private readonly IRepositoryFactory repoFactory;

        public UsersDal(ILogger<UsersDal> logger,
            IRepositoryFactory repoFactory)
        {
            Require.NotNull(logger, nameof(logger));
            Require.NotNull(repoFactory, nameof(repoFactory));

            this.logger = logger;
            this.repoFactory = repoFactory;
        }

        public void SaveUsers(IEnumerable<AdUser> users, int pollingId)
        {
            Require.NotNull(users, nameof(users));

            logger.LogInformation("Start saving all users");

            using (var context = repoFactory.Create<Contracts.Model.AdUser>())
            {
                var dbUsers = users
                    .OrderBy(u => u.Sid)
                    .Select(Transform)
                    .ForEachLazy(u =>
                    {
                        u.PollingId = pollingId;
                    });

                var perfWatch = Stopwatch.StartNew();
                context.AddRange(dbUsers);
                perfWatch.Stop();
                logger.LogInformation("Adding users to context took: {Elapsed}", perfWatch.Elapsed);

                perfWatch = Stopwatch.StartNew();
                var affected = context.SaveChanges();
                perfWatch.Stop();
                logger.LogInformation("Saving users took: {Elapsed}. Affected rows: {Affected}", perfWatch.Elapsed, affected);
            }
        }

        public IEnumerable<IAdUser> GetFromPolling(int pollingId)
        {
            using (var context = repoFactory.Create<Contracts.Model.AdUser>())
            {
                return context.Get()
                    .Where(u => u.PollingId == pollingId)
                    .ToArray();
            }
        }

        private Contracts.Model.AdUser Transform(AdUser user)
        {
            return new Contracts.Model.AdUser
            {
                AccountExpirationDate = user.AccountExpirationDate,
                AccountLockoutTime = user.AccountLockoutTime,
                AccountName = user.AccountName,
                Description = user.Description,
                Disabled = user.Disabled,
                Dn = user.Dn,
                Domain = user.Domain,
                Name = user.Name,
                Sid = user.Sid,
                Surname = user.Surname,
                Manager = user.Manager,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                DelegationPermitted = user.DelegationPermitted,
                Guid = user.Guid,
                LastBadPasswordAttempt = user.LastBadPasswordAttempt,
                LastLogon = user.LastLogon,
                LastPasswordSet = user.LastPasswordSet,
                PasswordNeverExpires = user.PasswordNeverExpires,
                PasswordNotRequired = user.PasswordNotRequired,
                UserCannotChangePassword = user.UserCannotChangePassword
            };
        }
    }
}
