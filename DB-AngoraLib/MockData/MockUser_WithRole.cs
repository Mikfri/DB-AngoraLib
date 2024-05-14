using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockUserWithRole
    {
        public User User { get; set; }
        public string Role { get; set; }

        public MockUserWithRole(User user, string role)
        {
            User = user;
            Role = role;
        }
    }
}
