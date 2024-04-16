using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class MotherHood
    {
        //[ForeignKey("Rabbit")]
        public int MotherId { get; set; }
        Rabbit Rabbit { get; set; }

        //[ForeignKey("Rabbit")]
        public int ChildId { get; set; }
    }
}
