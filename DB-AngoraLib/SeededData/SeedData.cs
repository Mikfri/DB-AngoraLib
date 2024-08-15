using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.SeededData
{
    public static class SeedData
    {
        public static async Task Initialize(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Define roles and claims
            var roles = new List<(string roleName, List<string> claims)>
            {
                ("Admin", new List<string> { "ManageUsers", "ManageRoles" }),
                ("Moderator", new List<string> { "ApproveContent", "BanUsers" }),
                ("Breeder", new List<string> { "ManageBreederProfile", "ListRabbitsForSale" })
            };

            foreach (var (roleName, claims) in roles)
            {
                // Check if role exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Create role
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);

                    // Add claims to role
                    foreach (var claim in claims)
                    {
                        await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", claim));
                    }
                }
            }

            // Seed mock users
            var mockUsers = MockUsers.GetMockUsersWithRoles();
            foreach (var mockUserWithRole in mockUsers)
            {
                var user = mockUserWithRole.User;
                var rolesForUser = mockUserWithRole.Roles;

                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        UserName = user.Email,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber
                    };

                    var result = await userManager.CreateAsync(newUser, user.PasswordHash);
                    if (result.Succeeded)
                    {
                        foreach (var role in rolesForUser)
                        {
                            await userManager.AddToRoleAsync(newUser, role);
                        }
                    }
                }
            }
        }
    }
}
