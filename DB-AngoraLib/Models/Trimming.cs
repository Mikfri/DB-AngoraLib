using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Trimming
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Rabbit")]
        public int RabbitId { get; set; }
        public Rabbit Rabbit { get; set; }

        public DateOnly DateTrimmed { get; set; }

        public int? DaysSinceLastTrim { get; set; } // todo: en værdi som løbende ændrer sig ud fra DateTrimmed

        public int FirstSortmentWeightGram { get; set; }

        public int SecondSortmentWeightGram { get; set; }

        public int DisposableWoolWeightGram { get; set; }

        public int? TimeUsedMinutes { get; set; }

        public float? HairLengthCm { get; set; }

        public float? WoolDensity { get; set; }
        
    }
}
