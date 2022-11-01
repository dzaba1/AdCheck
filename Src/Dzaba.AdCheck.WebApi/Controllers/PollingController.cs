using Dzaba.AdCheck.ActiveDirectory;
using Dzaba.AdCheck.DataAccess.Contracts;
using Dzaba.AdCheck.Diff.Contracts;
using Dzaba.AdCheck.WebApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dzaba.AdCheck.WebApi.Controllers
{
    [Authorize(Roles = Roles.ReadOnly)]
    [ApiController]
    [Route("[controller]")]
    public class PollingController : ControllerBase
    {
        private readonly IPollingDal pollingDal;
        private readonly IUsersDal usersDal;

        public PollingController(IPollingDal pollingDal,
            IUsersDal usersDal)
        {
            this.pollingDal = pollingDal;
            this.usersDal = usersDal;
        }

        [HttpGet]
        public IEnumerable<IPolling> Get()
        {
            return pollingDal.GetAll().OrderByDescending(p => p.TimeStamp);
        }

        [HttpGet("{id}/users")]
        public IEnumerable<UserWithDn> GetUsers(int id)
        {
            return usersDal.GetFromPolling(id)
                .Select(u => new UserWithDn
                {
                    DnTokens = Dn.Parse(u.Dn),
                    User = u
                });
        }
    }
}
