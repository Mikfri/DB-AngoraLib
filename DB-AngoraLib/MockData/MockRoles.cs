using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockRoles
    {
        private static readonly List<IdentityRole> _rolesList = new List<IdentityRole>
        {
            new IdentityRole { Id = "AdminRoleId", Name = "Admin" },
            new IdentityRole { Id = "ModeratorRoleId", Name = "Moderator" },
            new IdentityRole { Id = "BreederRoleId", Name = "Breeder" },
            new IdentityRole { Id = "GuestRoleId", Name = "Guest" }
        };

        public static List<IdentityRole> GetMockRoles()
        {
            return _rolesList;
        }
    }
}
