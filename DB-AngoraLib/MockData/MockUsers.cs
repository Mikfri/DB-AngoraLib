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
            //-----------------: USER "BREEDER" :-----------------
            new MockUserWithRole(
            new Breeder
            {
                Id = "IdasId",
                FirstName = "Ida",
                LastName = "Friborg",
                RoadNameAndNo = "Fynsvej 14",
                ZipCode = 4060,
                City = "Kirke Såby",
                Email = "IdaFriborg87@gmail.com",
                PhoneNumber = "27586455",
                PasswordHash = "Ida123!",
                BreederRegNo = "5095",
                UserName = "IdaFriborg87@gmail.com", // Set UserName explicitly
                NormalizedUserName = "IDAFRIBORG87@GMAIL.COM", // Set NormalizedUserName explicitly
                NormalizedEmail = "IDAFRIBORG87@GMAIL.COM", // Set NormalizedEmail explicitly
                BreederBrand = new BreederBrand
                {
                    Id = 1,
                    UserId = "IdasId",
                    BreederBrandName = "Den brogede Angora",
                    BreederBrandDescription = "Satin angora opdrætter, og producent af uld i forskellige plantefarver",
                    BreederBrandLogo = null,
                    IsFindable = true,
                    //RabbitsForSale = MockRabbits.GetMockRabbits().Where(r => r.OwnerId == "IdasId" && r.ForSale == IsPublic.Ja).ToList(),
                    //RabbitsForBreeding = MockRabbits.GetMockRabbits().Where(r => r.OwnerId == "IdasId" && r.ForBreeding == IsPublic.Ja).ToList(),
                    //PublicWools = new List<Wool>() // Initialize with empty list or mock data
                },
                RabbitsOwned = MockRabbits.GetMockRabbits().Where(r => r.OwnerId == "IdasId").ToList(),
                RabbitsLinked = MockRabbits.GetMockRabbits().Where(r => r.OriginId == "IdasId").ToList()
            },
            new List<string> { "Moderator" }
            ),
            new MockUserWithRole(
            new Breeder
            {
                Id = "MajasId",
                FirstName = "Maja",
                LastName = "Hulstrøm",
                RoadNameAndNo = "Sletten 4",
                ZipCode = 4100,
                City = "Benløse",
                Email = "MajaJoensen89@gmail.com",
                PhoneNumber = "28733085",
                PasswordHash = "Maja123!",
                BreederRegNo = "5053",
                UserName = "MajaJoensen89@gmail.com", // Set UserName explicitly
                NormalizedUserName = "MAJAJOENSEN89@GMAIL.COM", // Set NormalizedUserName explicitly
                NormalizedEmail = "MAJAJOENSEN89@GMAIL.COM", // Set NormalizedEmail explicitly
                BreederBrand = new BreederBrand
                {
                    Id = 2,
                    UserId = "MajasId",
                    BreederBrandName = "Slettens Angora",
                    BreederBrandDescription = "Oprdræt af Satin angoraer, samt salg af skind og uld",
                    BreederBrandLogo = "logo.png",
                    IsFindable = true,
                    //RabbitsForSale = MockRabbits.GetMockRabbits().Where(r => r.OwnerId == "MajasId" && r.ForSale == IsPublic.Ja).ToList(),
                    //RabbitsForBreeding = MockRabbits.GetMockRabbits().Where(r => r.OwnerId == "MajasId" && r.ForBreeding == IsPublic.Ja).ToList(),
                    //PublicWools = new List<Wool>() // Initialize with empty list or mock data
                },
                RabbitsOwned = MockRabbits.GetMockRabbits().Where(r => r.OwnerId == "MajasId").ToList(),
                RabbitsLinked = MockRabbits.GetMockRabbits().Where(r => r.OriginId == "MajasId").ToList()
            },
            new List<string> { "Breeder" }
            ),
            //-----------------: USER :-----------------
            new MockUserWithRole(
            new User
            {
                Id = "MikkelsId",
                FirstName = "Mikkel",
                LastName = "Friborg",
                RoadNameAndNo = "Fynsvej 14",
                ZipCode = 4060,
                City = "Kirke Såby",
                Email = "Mikk.fri@gmail.com",
                PhoneNumber = "81183394",
                PasswordHash = "Mikkel123!"
            },
            new List<string> { "Admin" }
            ),
        };

        public static List<MockUserWithRole> GetMockUsersWithRoles()
        {
            return _usersWithRolesList ?? new List<MockUserWithRole>();
        }
    }
}
