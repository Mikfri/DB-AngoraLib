using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockUserClaims
    {
        public static List<Claim> GetMockUserClaimsForUser(User user)
        {
            var userClaims = new List<Claim>();

            // Tilføj en speciel tilladelse for Maja
            if (user.FirstName == "Maja")
            {
                userClaims.Add(new Claim("SpecialPermission", "Update_Any_Rabbit"));
            }

            return userClaims;
        }
    }
}
