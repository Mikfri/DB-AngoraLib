﻿using DB_AngoraLib.Services.RoleService;
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

        public string? BreederRegNo { get; set; } // unik property for Breeder role, med unik string
        //public string? MemberNo { get; set; }     // unik property for Member role, med unik string
        //2 første numre = forneing = 3 nummer = forennings type = sidste to single eller familie medlem
        public BreederBrand? BreederBrand { get; set; } // navigation property til BreederBrand
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool PublicProfile { get; set; } = false; // default false, så brugerne selv skal vælge at gøre deres profil offentlig

        public string RoadNameAndNo { get; set; }
        public int ZipCode { get; set; }
        public string City { get; set; }
        
        [NotMapped]
        public string Password { get; set; }    // hack: For vi via MockUsers kan sætte password uden at det bliver hashet


        //public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        //public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        //public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
        //public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }

        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<ApplicationBreeder> BreederApplications { get; set; }

        public virtual ICollection<Rabbit> RabbitsOwned { get; set; }
        public virtual ICollection<Rabbit> RabbitsLinked { get; set; }  //RabbitsRaised?

        public virtual ICollection<TransferRequst> RabbitTransfers_Issued { get; set; }
        public virtual ICollection<TransferRequst> RabbitTransfers_Received { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }


        public User(string? breederRegNo, string firstName, string lastName, string roadName, int zipCode, string city, string email, string phone, string password)
        {
            BreederRegNo = breederRegNo;
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
        public User() 
        {
            BreederApplications = new List<ApplicationBreeder>();
            RabbitsOwned = new List<Rabbit>();
            RabbitsLinked = new List<Rabbit>();
            RabbitTransfers_Issued = new List<TransferRequst>();
            RabbitTransfers_Received = new List<TransferRequst>();
            RefreshTokens = new List<RefreshToken>();
        }
    }
}
