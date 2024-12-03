using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DB_AngoraLib.SeededData
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var hasher = new PasswordHasher<User>();

            // Seed Users
            var breeder1 = new Breeder
            {
                Id = "IdasId",
                BreederRegNo = "5095",
                FirstName = "Ida",
                LastName = "Friborg",
                RoadNameAndNo = "Fynsvej 14",
                ZipCode = 4060,
                City = "Kirke Såby",
                Email = "IdaFriborg87@gmail.com",
                PhoneNumber = "27586455",
                UserName = "IdaFriborg87@gmail.com",
                NormalizedUserName = "IDAFRIBORG87@GMAIL.COM",
                NormalizedEmail = "IDAFRIBORG87@GMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "Ida123!")
            };

            var breeder2 = new Breeder
            {
                Id = "MajasId",
                FirstName = "Maja",
                BreederRegNo = "5053",
                LastName = "Hulstrøm",
                RoadNameAndNo = "Sletten 4",
                ZipCode = 4100,
                City = "Benløse",
                Email = "MajaJoensen89@gmail.com",
                PhoneNumber = "28733085",
                UserName = "MajaJoensen89@gmail.com",
                NormalizedUserName = "MAJAJOENSEN89@GMAIL.COM",
                NormalizedEmail = "MAJAJOENSEN89@GMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "Maja123!")
            };

            var user = new User
            {
                Id = "MikkelsId",
                FirstName = "Mikkel",
                LastName = "Friborg",
                RoadNameAndNo = "Fynsvej 14",
                ZipCode = 4060,
                City = "Kirke Såby",
                Email = "Mikk.fri@gmail.com",
                PhoneNumber = "81183394",
                UserName = "Mikk.fri@gmail.com",
                NormalizedUserName = "MIKK.FRI@GMAIL.COM",
                NormalizedEmail = "MIKK.FRI@GMAIL.COM",
                PasswordHash = hasher.HashPassword(null, "Mikk123!")
            };

            modelBuilder.Entity<Breeder>().HasData(breeder1, breeder2);
            modelBuilder.Entity<User>().HasData(user);

            // Seed Roles
            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Moderator", NormalizedName = "MODERATOR" },
                new IdentityRole { Id = "3", Name = "BreederPremium", NormalizedName = "BREEDERPREMIUM" },
                new IdentityRole { Id = "4", Name = "BreederBasic", NormalizedName = "BREEDERBASIC" },
                new IdentityRole { Id = "5", Name = "UserBasicFree", NormalizedName = "USERBASICFREE" }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);

            // Seed UserRoles
            var userRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> { UserId = "IdasId", RoleId = "2" }, // Moderator
                new IdentityUserRole<string> { UserId = "MajasId", RoleId = "3" }, // BreederPremium
                new IdentityUserRole<string> { UserId = "MikkelsId", RoleId = "1" }, // Admin
                // Tilføjelse af flere roller:
                //new IdentityUserRole<string> { UserId = "MajasId", RoleId = "2" }
            };

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRoles);

            // Seed RoleClaims
            var roleClaims = new List<IdentityRoleClaim<string>>
            {
                // Admin Claims
                new IdentityRoleClaim<string> { Id = 1, RoleId = "1", ClaimType = "User:Create", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 2, RoleId = "1", ClaimType = "User:Read", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 3, RoleId = "1", ClaimType = "User:Update", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 4, RoleId = "1", ClaimType = "User:Delete", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 5, RoleId = "1", ClaimType = "Rabbit:Create", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 6, RoleId = "1", ClaimType = "Rabbit:Read", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 7, RoleId = "1", ClaimType = "Rabbit:Update", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 8, RoleId = "1", ClaimType = "Rabbit:Delete", ClaimValue = "Any" },

                // Moderator Claims
                new IdentityRoleClaim<string> { Id = 9, RoleId = "2", ClaimType = "Rabbit:Create", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 10, RoleId = "2", ClaimType = "Rabbit:Read", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 11, RoleId = "2", ClaimType = "Rabbit:Update", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 12, RoleId = "2", ClaimType = "Rabbit:Delete", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 23, RoleId = "2", ClaimType = "User:Read", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 24, RoleId = "2", ClaimType = "User:Update", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 25, RoleId = "2", ClaimType = "User:Create", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 26, RoleId = "2", ClaimType = "User:Delete", ClaimValue = "Own" },

                // BreederPremium Claims
                new IdentityRoleClaim<string> { Id = 13, RoleId = "3", ClaimType = "Rabbit:Create", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 14, RoleId = "3", ClaimType = "Rabbit:Read", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 15, RoleId = "3", ClaimType = "Rabbit:Update", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 16, RoleId = "3", ClaimType = "Rabbit:Delete", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 17, RoleId = "3", ClaimType = "Rabbit:ImageCount", ClaimValue = "3" },
                new IdentityRoleClaim<string> { Id = 27, RoleId = "3", ClaimType = "User:Read", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 28, RoleId = "3", ClaimType = "User:Update", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 29, RoleId = "3", ClaimType = "User:Create", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 30, RoleId = "3", ClaimType = "User:Delete", ClaimValue = "Own" },

                // BreederBasic Claims
                new IdentityRoleClaim<string> { Id = 18, RoleId = "4", ClaimType = "Rabbit:Create", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 19, RoleId = "4", ClaimType = "Rabbit:Read", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 20, RoleId = "4", ClaimType = "Rabbit:Update", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 21, RoleId = "4", ClaimType = "Rabbit:Delete", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 22, RoleId = "4", ClaimType = "Rabbit:ImageCount", ClaimValue = "1" },
                new IdentityRoleClaim<string> { Id = 31, RoleId = "4", ClaimType = "User:Read", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 32, RoleId = "4", ClaimType = "User:Update", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 33, RoleId = "4", ClaimType = "User:Create", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 34, RoleId = "4", ClaimType = "User:Delete", ClaimValue = "Own" }
            };

            modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(roleClaims);


            // Seed Rabbits
            var rabbits = MockRabbits.GetMockRabbits();
            modelBuilder.Entity<Rabbit>().HasData(rabbits);

            // Seed BreederBrands
            var breederBrands = new List<BreederBrand>
            {
                new BreederBrand
                {
                    Id = 1,
                    UserId = "IdasId",
                    BreederBrandName = "Friborg's kaninavl",
                    BreederBrandDescription = "Lille opdræt af Satin-angoraer i forskellige farver. Jeg tilbyder god uld med og uden plantefarver",
                    BreederBrandLogo = null,
                    IsFindable = true
                },
                new BreederBrand
                {
                    Id = 2,
                    UserId = "MajasId",
                    BreederBrandName = "Sletten's kaninavl",
                    BreederBrandDescription = "Jeg tilbyder Satin-angoraer, uld og skind i klassiske farver",
                    BreederBrandLogo = null,
                    IsFindable = true
                }
            };

            modelBuilder.Entity<BreederBrand>().HasData(breederBrands);
        }
    }
}
