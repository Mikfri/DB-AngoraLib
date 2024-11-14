using DB_AngoraLib.DTOs;
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

        public async Task AddUserClaimAsync(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, claim);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to add claim to user.");
            }
        }

        public async Task RemoveUserClaimAsync(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.RemoveClaimAsync(user, claim);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to remove claim from user.");
            }
        }

        public async Task<User_RolesAndClaimsDTO> GetUserRolesAndClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);   // Henter navnene på brugerens roller
            var claims = await _userManager.GetClaimsAsync(user); // Henter UserClaims og RoleClaims

            var userRolesAndClaims = new User_RolesAndClaimsDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.ToList(),
                Claims = claims.Select(c => new ClaimDTO { Type = c.Type, Value = c.Value }).ToList()
            };

            return userRolesAndClaims;
        }

        public async Task UpgradeUserRoleAsync(string userId, string newRoleId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            // Remove old roles
            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);

            // Add new role
            var newRole = await _roleManager.FindByIdAsync(newRoleId);
            if (newRole == null) throw new Exception("Role not found");

            await _userManager.AddToRoleAsync(user, newRole.Name);
        }

        //public async Task RemoveSpecialPermissionFromUser(string userId, string claimValue)
        //{
        //    // Check if the claimValue exists in RoleClaims
        //    if (!RoleClaims.Get_AspNetRoleClaims().Any(rc => rc.ClaimValue == claimValue))
        //    {
        //        throw new ArgumentException("Invalid claim value.");
        //    }

        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        throw new ArgumentException("User not found.");
        //    }

        //    var claim = new Claim("SpecialPermission", claimValue);
        //    var result = await _userManager.RemoveClaimAsync(user, claim);

        //    if (!result.Succeeded)
        //    {
        //        throw new Exception("Failed to remove special permission from user.");
        //    }
        //}




    }
}
