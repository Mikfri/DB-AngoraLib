using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace DB_AngoraLib.MockData
{
    public class MockRoleClaims
    {
        public static List<IdentityRoleClaim<string>> GetMockRoleClaimsForRole(string roleName)
        {
            var roleClaims = new List<IdentityRoleClaim<string>>();
            var roleId = MockRoles.GetMockRoles().First(r => r.Name == roleName).Id;
            var claimsForRole = RoleClaims.Get_AspNetRoleClaims().Where(rc => rc.RoleId == roleId);

            foreach (var claim in claimsForRole)
            {
                roleClaims.Add(new IdentityRoleClaim<string>
                {
                    RoleId = roleId,
                    ClaimType = claim.ClaimType,
                    ClaimValue = claim.ClaimValue
                });
            }
            return roleClaims;
        }
    }

}
