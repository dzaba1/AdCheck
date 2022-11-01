using System.Linq;
using Dzaba.AdCheck.DataAccess.Contracts;
using Dzaba.AdCheck.DataAccess.Contracts.Model;
using Dzaba.AdCheck.DataAccess.Repository;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.Logging;

namespace Dzaba.AdCheck.DataAccess.Dal
{
    internal sealed class RolesDal : IRolesDal
    {
        private readonly ILogger<RolesDal> logger;
        private readonly IRepositoryFactory repoFactory;

        public RolesDal(ILogger<RolesDal> logger,
            IRepositoryFactory repoFactory)
        {
            Require.NotNull(logger, nameof(logger));
            Require.NotNull(repoFactory, nameof(repoFactory));

            this.logger = logger;
            this.repoFactory = repoFactory;
        }

        public string GetRoleForUser(string name)
        {
            Require.NotWhiteSpace(name, nameof(name));

            logger.LogInformation("Getting role for user '{User}'", name);

            using (var context = repoFactory.Create<UserRole>())
            {
                var query = from u in context.Get()
                    join r in context.GetOther<Role>() on u.RoleId equals r.Id
                    where u.UserName.ToLower() == name.ToLower()
                    select r.Name;

                return query.FirstOrDefault();
            }
        }
    }
}
