using Dzaba.AdCheck.Diff.Contracts;
using Dzaba.AdCheck.Utils;
using Dzaba.AdCheck.WebApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dzaba.AdCheck.WebApi.Controllers
{
    [Authorize(Roles = Roles.ReadOnly)]
    [ApiController]
    [Route("[controller]")]
    public class DiffController : ControllerBase
    {
        private readonly IDiffer differ;

        public DiffController(IDiffer differ)
        {
            Require.NotNull(differ, nameof(differ));

            this.differ = differ;
        }

        [HttpGet("{leftPollingId}/{rightPollingId}")]
        public IEnumerable<DiffReport> Get(int leftPollingId, int rightPollingId)
        {
            return differ.Diff(leftPollingId, rightPollingId, null);
        }

        [HttpPost("{leftPollingId}/{rightPollingId}")]
        public IEnumerable<DiffReport> GetWithFilter(int leftPollingId, int rightPollingId, [FromBody] DiffFiltering filtering)
        {
            return differ.Diff(leftPollingId, rightPollingId, filtering);
        }
    }
}
