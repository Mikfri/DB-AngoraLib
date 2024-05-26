using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockDataInitializer
    {
        private readonly DB_AngoraContext _context;
        private readonly UserManager<User> _userManager;

        public MockDataInitializer(DB_AngoraContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void Initialize()
        {
            var mockUsersWithRoles = MockUsers.GetMockUsersWithRoles();
            foreach (var mockUserWithRole in mockUsersWithRoles)
            {
                AddUserWithClaimsAndRoles(mockUserWithRole);
            }

            var mockRabbits = MockRabbits.GetMockRabbits();
            _context.Rabbits.AddRange(mockRabbits);
            _context.SaveChanges();
        }

        private void AddUserWithClaimsAndRoles(MockUserWithRole mockUserWithRole)
        {
            _context.Users.Add(mockUserWithRole.User);

            AddStandardClaims(mockUserWithRole.User);
            AddUserClaims(mockUserWithRole.User);
            AddRoleClaims(mockUserWithRole);
        }

        private void AddStandardClaims(User user)
        {
            var standardClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
            };
            _context.UserClaims.AddRange(standardClaims.Select(sc => new IdentityUserClaim<string>
            {
                UserId = user.Id,
                ClaimType = sc.Type,
                ClaimValue = sc.Value
            }));
        }

        private void AddUserClaims(User user)
        {
            var userClaims = MockUserClaims.GetMockUserClaimsForUser(user);
            _context.UserClaims.AddRange(userClaims.Select(uc => new IdentityUserClaim<string>
            {
                UserId = user.Id,
                ClaimType = uc.Type,
                ClaimValue = uc.Value
            }));
        }

        private void AddRoleClaims(MockUserWithRole mockUserWithRole)
        {
            foreach (var role in mockUserWithRole.Roles)
            {
                // Tilføj Role til User
                _userManager.AddToRoleAsync(mockUserWithRole.User, role).GetAwaiter().GetResult();

                // Tilføj Role som Claim
                _context.UserClaims.Add(new IdentityUserClaim<string>
                {
                    UserId = mockUserWithRole.User.Id,
                    ClaimType = ClaimTypes.Role,
                    ClaimValue = role
                });

                var roleClaims = MockRoleClaims.GetMockRoleClaimsForRole(role);
                foreach (var roleClaim in roleClaims)
                {
                    _context.UserClaims.Add(new IdentityUserClaim<string>
                    {
                        UserId = mockUserWithRole.User.Id,
                        ClaimType = roleClaim.ClaimType,
                        ClaimValue = roleClaim.ClaimValue
                    });
                }
            }
        }
    }

}
