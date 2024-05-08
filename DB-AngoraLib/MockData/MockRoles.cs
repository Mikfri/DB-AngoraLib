using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockRoles
    {
        private static readonly List<string> _rolesList = new List<string>
        {
            "Admin",
            "Moderator",
            "Breeder",
            "Guest"
        };

        public static List<string> GetMockRoles()
        {
            return _rolesList;
        }
    }
}
