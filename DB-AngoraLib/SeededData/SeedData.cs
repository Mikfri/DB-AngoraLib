using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.SeededData
{
    public static class SeedData
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Seed Users
            var breeder1 = new Breeder
            {
                Id = "IdasId",
                FirstName = "Ida",
                LastName = "Friborg",
                RoadNameAndNo = "Fynsvej 14",
                ZipCode = 4060,
                City = "Kirke Såby",
                Email = "IdaFriborg87@gmail.com",
                PhoneNumber = "27586455",
                Password = "Ida123!",
                BreederRegNo = "5095",
                UserName = "IdaFriborg87@gmail.com",
                NormalizedUserName = "IDAFRIBORG87@GMAIL.COM",
                NormalizedEmail = "IDAFRIBORG87@GMAIL.COM"
            };

            var breeder2 = new Breeder
            {
                Id = "MajasId",
                FirstName = "Maja",
                LastName = "Hulstrøm",
                RoadNameAndNo = "Sletten 4",
                ZipCode = 4100,
                City = "Benløse",
                Email = "MajaJoensen89@gmail.com",
                PhoneNumber = "28733085",
                Password = "Maja123!",
                BreederRegNo = "5053",
                UserName = "MajaJoensen89@gmail.com",
                NormalizedUserName = "MAJAJOENSEN89@GMAIL.COM",
                NormalizedEmail = "MAJAJOENSEN89@GMAIL.COM"
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
                Password = "Mikkel123!"
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
        }
    }
}
