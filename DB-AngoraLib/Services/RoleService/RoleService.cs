using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RoleService
{
    //public enum Role
    //{
    //    Admin,
    //    Manager,
    //    Breeder,
    //    Guest
    //}

    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task UpgradeUserToBreeder(User user, string breederRegNo)
        {
            user.BreederRegNo = breederRegNo;
            await _userManager.UpdateAsync(user);
            await _userManager.RemoveFromRoleAsync(user, "Guest");
            await _userManager.AddToRoleAsync(user, "Breeder");
        }

        /// <summary>
        /// Denne metode kan returnere en liste over alle eksisterende roller
        /// </summary>
        /// <returns></returns>
        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task CreateRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task AssignRoleAsync(User user, string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

        /// <summary>
        /// Denne metode kan opdatere en eksisterende rolle. Den skal først finde rollen, opdatere dens egenskaber,
        /// og derefter gemme ændringerne.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateRoleAsync(string oldRoleName, string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(oldRoleName);
            if (role != null)
            {
                role.Name = newRoleName;
                await _roleManager.UpdateAsync(role);
            }
        }


        /// <summary>
        /// Denne metode kan bruges til at slette en eksisterende rolle.
        /// Den skal først tjekke, om rollen eksisterer, og derefter slette den.
        /// </summary>
        /// <param name="roleName">Enum baseret</param>
        /// <returns></returns>
        public async Task DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
        }
    }
}
