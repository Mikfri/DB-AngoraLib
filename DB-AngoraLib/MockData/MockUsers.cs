using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockUsers
    {
        private static List<MockUserWithRole> _usersWithRolesList = new List<MockUserWithRole>()
        {
            new MockUserWithRole(
            new User(
            "5095",
            "Ida",
            "Friborg",
            "Fynsvej 14",
            4060,
            "Kirke Såby",
            "IdaFriborg87@gmail.com",
            "27586455",
            "Ida123!")
            {
                Id = "IdasId"
            },
            "Moderator"
            ),

            new MockUserWithRole(
            new User(
            "5053",
            "Maja",
            "Hulstrøm",
            "Sletten 4",
            4100,
            "Benløse",
            "MajaJoensen89@gmail.com",
            "28733085",
            "Maja123!")
            {
                Id = "MajasId"
            },
            "Breeder"
            ),

            new MockUserWithRole(
            new User(
            null,
            "Mikkel",
            "Friborg",
            "Fynsvej 14",
            4060,
            "Kirke Såby",
            "Mikk.fri@gmail.com",
            "81183394",
            "Mikkel123!")
            {
                Id = "MikkelsId"
            },
            "Admin"
            ),
        };

        //public static List<MockUserWithRole> GetMockUsersWithRoles()
        //{ return _usersWithRolesList; }
        public static List<MockUserWithRole> GetMockUsersWithRoles()
        {
            return _usersWithRolesList ?? new List<MockUserWithRole>();
        }

        private static List<User> _usersList = new List<User>()
        {
            new User(
            "5095",
            "Ida",
            "Friborg",
            "Fynsvej 14",
            4060,
            "Kirke Såby",
            "IdaFriborg87@gmail.com",
            "27586455",
            "Ida123!")
            {
                Id = "IdasId"
            },

            new User(
            "5053",
            "Maja",
            "Hulstrøm",
            "Sletten 4",
            4100,
            "Benløse",
            "MajaJoensen89@gmail.com",
            "28733085",
            "Maja123!")
            {
                Id = "MajasId"
            },

            new User(
            null,
            "Mikkel",
            "Friborg",
            "Fynsvej 14",
            4060,
            "Kirke Såby",
            "Mikk.fri@gmail.com",
            "81183394",
            "Mikkel123!")
            {
                Id = "MikkelsId"
            },
        };

        public static List<User> GetMockUsers()
        { return _usersList; }
    }
}
