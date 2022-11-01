using System.Security.Claims;
using Dzaba.AdCheck.DataAccess.Contracts;
using Dzaba.AdCheck.Utils;
using Microsoft.AspNetCore.Authentication;

namespace Dzaba.AdCheck.WebApi.Auth
{
    internal sealed class RolesTransformer : IClaimsTransformation
    {
        private readonly IRolesDal rolesDal;

        public RolesTransformer(IRolesDal rolesDal)
        {
            Require.NotNull(rolesDal, nameof(rolesDal));

            this.rolesDal = rolesDal;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var roleClaims = GetRoles(principal.Identity.Name)
                .Select(r => new Claim(claimsIdentity.RoleClaimType, r));

            claimsIdentity.AddClaims(roleClaims);

            return await Task.FromResult(principal);
        }

        private IEnumerable<string> GetRoles(string userName)
        {
            var role = rolesDal.GetRoleForUser(userName);
            if (string.Equals(Roles.Admin, role, StringComparison.OrdinalIgnoreCase))
            {
                yield return Roles.Admin;
                yield return Roles.ReadOnly;
                yield break;
            }

            if (string.Equals(Roles.ReadOnly, role, StringComparison.OrdinalIgnoreCase))
            {
                yield return Roles.ReadOnly;
            }
        }
    }
}
