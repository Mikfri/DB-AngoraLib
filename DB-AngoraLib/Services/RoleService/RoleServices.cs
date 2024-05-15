using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RoleService
{
    public class RoleServices : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleServices(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task AdminMethod_AddClaimToRole(string roleName, string claimType, string claimValue)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claim = new Claim(claimType, claimValue);
                var result = await _roleManager.AddClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    // Handle error
                }
            }
        }

        public async Task AdminMethod_RemoveClaimFromRole(string roleName, string claimType, string claimValue)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claim = new Claim(claimType, claimValue);
                await _roleManager.RemoveClaimAsync(role, claim);
            }
        }

        public async Task AddClaimToUser(User user, string claimType, string claimValue)
        {
            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, claim);
            if (!result.Succeeded)
            {
                // Handle error
            }
        }

        public async Task ModMethod_AssignUserToRole(User user, string roleName)
        {
            if (roleName != "Admin" && roleName != "Moderator")
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
        public async Task ModMethod_RemoveUserFromRole(User user, string roleName)
        {
            if (roleName != "Admin" && roleName != "Moderator")
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }
        }

        public async Task ModMethod_AssignUserToRole_WithRoleClaims(User user, string roleName)
        {
            if (roleName != "Admin" && roleName != "Moderator")
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);

                    // Get the role
                    var role = await _roleManager.FindByNameAsync(roleName);

                    // Get the claims associated with the role
                    var roleClaims = await _roleManager.GetClaimsAsync(role);

                    // Assign each claim to the user
                    foreach (var claim in roleClaims)
                    {
                        await _userManager.AddClaimAsync(user, claim);
                    }
                }
            }
        }
               


        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task AdminMethod_AssignRoleAsync(User user, string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

        public async Task AdminMethod_UpdateRoleNameAsync(string oldRoleName, string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(oldRoleName);
            if (role != null)
            {
                role.Name = newRoleName;
                role.NormalizedName = _roleManager.NormalizeKey(newRoleName);
                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    // Handle error
                }
            }
        }
    }
}
