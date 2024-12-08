using DB_AngoraLib.Services.RoleService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-8.0
    public class User : IdentityUser
    {

        //public string? MemberNo { get; set; }     // unik property for Member role, med unik string
        //2 første numre = forneing = 3 nummer = forennings type = sidste to single eller familie medlem
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string RoadNameAndNo { get; set; }
        public int ZipCode { get; set; }
        public string City { get; set; }
        public string? ProfilePicture { get; set; } // Path or URI to the profile picture


        [NotMapped]
        public string Password { get; set; }    // hack: For vi via MockUsers kan sætte password uden at det bliver hashet

        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }

        public virtual ICollection<ApplicationBreeder> BreederApplications { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }


        public User()
        {
            BreederApplications = new List<ApplicationBreeder>();
            RefreshTokens = new List<RefreshToken>();
        }

        public User(string firstName, string lastName, string roadName, int zipCode, string city, string email, string phone, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            RoadNameAndNo = roadName;
            ZipCode = zipCode;
            City = city;
            Email = email;
            NormalizedEmail = email.ToUpperInvariant();
            UserName = email; // IdentityUser uses UserName for login
            NormalizedUserName = email.ToUpperInvariant();
            PhoneNumber = phone;
            Password = password;
            //PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
