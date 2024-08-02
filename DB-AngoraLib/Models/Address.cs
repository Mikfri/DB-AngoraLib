using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string StreetNameAndNumber { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public Address() { }
    }
}
