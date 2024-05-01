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
        private readonly PasswordHasher<User> _passwordHasher;

        public MockUsers(PasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public List<User> GetMockUsers()
        {
            var usersList = new List<User> 
            {
                new User {
                Id = "5095",
                FirstName = "Ida",
                LastName = "Friborg",
                RoadNameAndNo = "Fynsvej 14",
                ZipCode = 4060, City = "Kirke Såby",
                Email = "IdaFribor87@gmail.com",
                PhoneNumber = "27586455",
                IsAdmin = true,
                PasswordHash = _passwordHasher.HashPassword(null, "Ida123")
                },
                new User {
                Id = "5053",
                FirstName = "Maja",
                LastName = "Hulstrøm",
                RoadNameAndNo = "Sletten 4",
                ZipCode = 4100,
                City = "Benløse",
                Email = "MajaJoensen89@gmail.com",
                PhoneNumber = "28733085",
                IsAdmin = false,
                PasswordHash = _passwordHasher.HashPassword(null, "Maja123")
                },
            };

            return usersList;
        }
    }
}
