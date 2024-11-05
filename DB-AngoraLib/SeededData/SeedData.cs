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
                PasswordHash = hasher.HashPassword(null, "Mikkel123!")
            };

            modelBuilder.Entity<Breeder>().HasData(breeder1, breeder2);
            modelBuilder.Entity<User>().HasData(user);

            // Seed Roles
            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "1", Name = "Moderator", NormalizedName = "MODERATOR" },
                new IdentityRole { Id = "2", Name = "Breeder", NormalizedName = "BREEDER" },
                new IdentityRole { Id = "3", Name = "Admin", NormalizedName = "ADMIN" }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);

            // Seed UserRoles
            var userRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> { UserId = "IdasId", RoleId = "1" },
                new IdentityUserRole<string> { UserId = "MajasId", RoleId = "2" },
                new IdentityUserRole<string> { UserId = "MikkelsId", RoleId = "3" }
            };

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRoles);

            // Seed RoleClaims
            var roleClaims = new List<IdentityRoleClaim<string>>
            {
                new IdentityRoleClaim<string> { Id = 1, RoleId = "3", ClaimType = "User:Read", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 2, RoleId = "3", ClaimType = "User:Create", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 3, RoleId = "3", ClaimType = "User:Update", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 4, RoleId = "3", ClaimType = "User:Delete", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 5, RoleId = "3", ClaimType = "Rabbit:Create", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 6, RoleId = "3", ClaimType = "Rabbit:Read", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 7, RoleId = "3", ClaimType = "Rabbit:Update", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 8, RoleId = "3", ClaimType = "Rabbit:Delete", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 9, RoleId = "1", ClaimType = "Rabbit:Create", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 10, RoleId = "1", ClaimType = "Rabbit:Read", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 11, RoleId = "1", ClaimType = "Rabbit:Update", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 12, RoleId = "1", ClaimType = "Rabbit:Delete", ClaimValue = "Any" },
                new IdentityRoleClaim<string> { Id = 13, RoleId = "2", ClaimType = "Rabbit:Create", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 14, RoleId = "2", ClaimType = "Rabbit:Read", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 15, RoleId = "2", ClaimType = "Rabbit:Update", ClaimValue = "Own" },
                new IdentityRoleClaim<string> { Id = 16, RoleId = "2", ClaimType = "Rabbit:Delete", ClaimValue = "Own" }
            };

            modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(roleClaims);

            // Seed Rabbits
            var rabbits = MockRabbits.GetMockRabbits();
            modelBuilder.Entity<Rabbit>().HasData(rabbits);
        }
    }
}
