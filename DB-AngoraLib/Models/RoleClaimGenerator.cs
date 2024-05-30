using DB_AngoraLib.MockData;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class RoleClaimsGenerator
    {
        //private List<IdentityRoleClaim<string>> _roleClaimsList = new List<IdentityRoleClaim<string>>();
        //private int _idCounter = -1;
        //public RoleClaimsGenerator()
        //{
        //    //------------------------: ADMIN :------------------------
        //    // User
        //    AddRoleClaim("Admin", "RolePermission", "Get_Any_User");
        //    AddRoleClaim("Admin", "RolePermission", "Update_Any_User");
        //    AddRoleClaim("Admin", "RolePermission", "Delete_Any_User");

        //    // Rabbit
        //    AddRoleClaim("Admin", "RolePermission", "Create_Any_Rabbit");
        //    AddRoleClaim("Admin", "RolePermission", "Get_Any_Rabbit");
        //    AddRoleClaim("Admin", "RolePermission", "Update_Any_Rabbit");
        //    AddRoleClaim("Admin", "RolePermission", "Delete_Any_Rabbit");

        //    //------------------------: MODERATOR :------------------------
        //    // Rabbit
        //    AddRoleClaim("Moderator", "RolePermission", "Create_Any_Rabbit");
        //    AddRoleClaim("Moderator", "RolePermission", "Get_Any_Rabbit");
        //    AddRoleClaim("Moderator", "RolePermission", "Update_Any_Rabbit");
        //    AddRoleClaim("Moderator", "RolePermission", "Delete_Any_Rabbit");

        //    //------------------------: BREEDER :------------------------
        //    // Rabbit
        //    AddRoleClaim("Breeder", "RolePermission", "Create_Own_Rabbit");
        //    AddRoleClaim("Breeder", "RolePermission", "Get_Own_Rabbit");
        //    AddRoleClaim("Breeder", "RolePermission", "Update_Own_Rabbit");
        //    AddRoleClaim("Breeder", "RolePermission", "Delete_Own_Rabbit");

        //    //------------------------: GUEST :------------------------
        //    // Rabbit
        //    AddRoleClaim("Guest", "RolePermission", "Get_Own_Rabbit");
        //}

        //private void AddRoleClaim(string roleName, string claimType, string claimValue)
        //{
        //    var roleId = MockRoles.GetMockRoles().First(r => r.Name == roleName).Id;
        //    _roleClaimsList.Add(new IdentityRoleClaim<string> { Id = _idCounter--, RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });
        //}

        //public List<IdentityRoleClaim<string>> GenerateRoleClaims()
        //{
        //    return _roleClaimsList;
        //}
    }

}
