using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class BreederBrand
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public Breeder BreederBrandOwner { get; set; } // Opdateret til at referere til Breeder

        public string? BreederBrandName { get; set; }
        public string? BreederBrandDescription { get; set; }
        public string? BreederBrandLogo { get; set; }
        public bool IsFindable { get; set; } // Angiver om Breeder er findbar på kortet

        public virtual ICollection<Rabbit> RabbitsForSale { get; set; } // Er brugeren ICollection af rabbit Where...
        public virtual ICollection<Rabbit> RabbitsForBreeding { get; set; } // Er brugeren ICollection af rabbit Where...
        public virtual ICollection<Wool> PublicWools { get; set; } // Er brugeren ICollection af wool Where...

        public BreederBrand()
        {
            RabbitsForSale = new List<Rabbit>();
            RabbitsForBreeding = new List<Rabbit>();
            PublicWools = new List<Wool>();
        }
    }
}
