using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RoleService
{
    public interface IRoleService
    {
        Task AdminMethod_AddClaimToRole(string roleName, string claimType, string claimValue);
        Task AdminMethod_RemoveClaimFromRole(string roleName, string claimType, string claimValue);
        Task AddClaimToUser(User user, string claimType, string claimValue);
        Task ModMethod_AssignUserToRole(User user, string roleName);
        Task ModMethod_RemoveUserFromRole(User user, string roleName);
        Task ModMethod_AssignUserToRole_WithRoleClaims(User user, string roleName);
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task AdminMethod_AssignRoleAsync(User user, string roleName);
        Task AdminMethod_UpdateRoleNameAsync(string oldRoleName, string newRoleName);
    }
}
