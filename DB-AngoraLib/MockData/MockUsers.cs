using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockUsers
    {
        private static List<User> _usersList = new List<User>()
        {
            new User(
            "5095",
            "Ida",
            "Friborg",
            "Fynsvej 14",
            4060,
            "Kirke Såby",
            "IdaFribor87@gmail.com",
            "27586455",
            "Ida123",
            true),

            new User(
            "5053",
            "Maja",
            "Hulstrøm",
            "Sletten 4",
            4100,
            "Benløse",
            "MajaJoensen89@gmail.com",
            "28733085",
            "Maja123",
            false),

        };

        public static List<User> GetMockUsers()
        { return _usersList; }

    }
}
