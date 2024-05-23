using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockUserClaims // TODO: Lav en seperat liste som AccountServices.Register_BasicUserAsync og MockUserClaims kan hente så de stemmer overens
    {
        public static List<Claim> GetMockUserClaimsForUser(MockUserWithRole mockUserWithRole)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, mockUserWithRole.User.Id),
                new Claim(ClaimTypes.Role, mockUserWithRole.Role), // Add the user's role as a claim
                new Claim(ClaimTypes.Name, $"{mockUserWithRole.User.FirstName} {mockUserWithRole.User.LastName}"),
                new Claim(ClaimTypes.Email, mockUserWithRole.User.Email),
                // Tilføj flere user-spesifikke claims her...
            };

            var role = MockRoles.GetMockRoles().First(r => r.Name == mockUserWithRole.Role);
            var roleClaims = MockRoleClaims.GetMockRoleClaims()
                .Where(rc => rc.RoleId == role.Id)
                .Select(rc => new Claim(rc.ClaimType, rc.ClaimValue));

            userClaims.AddRange(roleClaims);

            return userClaims;
        }
    }
}
