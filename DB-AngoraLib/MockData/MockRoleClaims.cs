using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockRoleClaims
    {
        private static List<string> full_Any_CRUD_Rabbit_RolePermits = new List<string> { "Admin", "Moderator" };
        private static List<string> full_Limited_CRUD_Rabbit_RolePermits = new List<string> { "Breeder" };

        private static List<IdentityRoleClaim<string>> _roleClaimsList = new List<IdentityRoleClaim<string>>();

        static MockRoleClaims()
        {
            foreach (var role in full_Any_CRUD_Rabbit_RolePermits)
            {
                var roleId = MockRoles.GetMockRoles().First(r => r.Name == role).Id;
                _roleClaimsList.AddRange(new List<IdentityRoleClaim<string>>
                {
                    new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = "RolePermission", ClaimValue = "Create_Any_Rabbit" },
                    new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = "RolePermission", ClaimValue = "Get_Any_Rabbit" },
                    new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = "RolePermission", ClaimValue = "Update_Any_Rabbit" },
                    new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = "RolePermission", ClaimValue = "Delete_Any_Rabbit" }
                });
            }

            foreach (var role in full_Limited_CRUD_Rabbit_RolePermits)
            {
                var roleId = MockRoles.GetMockRoles().First(r => r.Name == role).Id;
                _roleClaimsList.AddRange(new List<IdentityRoleClaim<string>>
                {
                    new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = "RolePermission", ClaimValue = "Create_Own_Rabbit" },
                    new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = "RolePermission", ClaimValue = "Get_Own_Rabbit" },
                    new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = "RolePermission", ClaimValue = "Update_Own_Rabbit" },
                    new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = "RolePermission", ClaimValue = "Delete_Own_Rabbit" }
                });
            }
        }

        public static List<IdentityRoleClaim<string>> GetMockRoleClaims()
        {
            return _roleClaimsList;
        }
    }

}
