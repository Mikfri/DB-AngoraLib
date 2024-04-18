using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Litter
    {
        public int Id { get; set; }

        public string MotherRightEarId { get; set; }
        public string MotherLeftEarId { get; set; }
        public virtual Rabbit? Mother { get; set; }

        public string FatherRightEarId { get; set; }
        public string FatherLeftEarId { get; set; }
        public virtual Rabbit? Father { get; set; }
        public virtual ICollection<Rabbit> Rabbits { get; set; }

        public Litter() { }
    }
}
