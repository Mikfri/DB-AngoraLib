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

        public int MotherId { get; set; }
        [ForeignKey("MotherId")]
        public virtual Rabbit Mother { get; set; }

        public int FatherId { get; set; }
        [ForeignKey("FatherId")]
        public virtual Rabbit Father { get; set; }

        public virtual ICollection<Rabbit> Rabbits { get; set; }
    }
}
