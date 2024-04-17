using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class User
    {
        public string BreederRegNo { get; set; }    // string! udelanske systemer(tyske heraf) benytter bogstaver.

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string RoadNameAndNo { get; set; }    // todo: class address måske?
        public int ZipCode { get; set; }
        public string City { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool? IsAdmin { get; set; }
        public string? Image {  get; set; }

        public ICollection<Rabbit> Rabbits { get; set; }


        public User(string breederRegNo, string firstName, string lastName, string roadName, int zipCode, string city, string email, string phone, string password, bool? isAdmin, string? image)
        {
            BreederRegNo = breederRegNo;
            FirstName = firstName;
            LastName = lastName;
            RoadNameAndNo = roadName;
            ZipCode = zipCode;
            City = city;
            Email = email;
            Phone = phone;
            Password = password;
            IsAdmin = isAdmin;
            Image = image;
        }

        public User() { }        

    }
}
