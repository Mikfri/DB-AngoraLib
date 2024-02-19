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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string RoadName { get; set; } // todo: class address måske?
        public string RoadNo { get; set; }  //7b? 33a? (string)
        public int ZipCode { get; set; }
        public string City { get; set; }    

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string Image {  get; set; }
    }
}
