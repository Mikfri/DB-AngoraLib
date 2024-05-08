using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string RoadNameAndNo { get; set; }
        public int ZipCode { get; set; }
        public string City { get; set; }

        [NotMapped]
        public string Password { get; set; }    // hack: For vi via MockUsers kan sætte password uden at det bliver hashet

        public bool? IsAdmin { get; set; }

        public ICollection<Rabbit> Rabbits { get; set; }

        public User(string breederRegNo, string firstName, string lastName, string roadName, int zipCode, string city, string email, string phone, string password, bool? isAdmin)
        {
            Id = breederRegNo;
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
            IsAdmin = isAdmin;
        }
        public User() { }
    }
}
