using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class BreederBrand   // TODO: Du skal arbejde på dens service og evt lave en 'MockBreederBrands'
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        //public string BreederFullName { get; set; } // redundant, as it is in User

        public string? BreederBrandName { get; set; }
        //public string BreederRegNo { get; set; } // redundant, as it is in User
        public string? BreederBrandDescription { get; set; }
        public string? BreederBrandLogo { get; set; }

        public BreederBrand() { }
    }
}
