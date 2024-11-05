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

            //// Seed RoleClaims
            //var roleClaims = RoleClaims.Get_AspNetRoleClaims();
            //modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(roleClaims);
        }
    }
}
