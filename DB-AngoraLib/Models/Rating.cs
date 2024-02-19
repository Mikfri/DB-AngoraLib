using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Rabbit")]
        public int RabbitId { get; set; }
        public Rabbit Rabbit { get; set; }

        public DateOnly DateRated { get; set; }
        public int WeightPoint { get; set; }
        public string? WeightNotice { get; set; } = null;
        public int BodyPoint { get; set; }
        public string? BodyNotice { get; set; } = null;
        public int FurPoint { get; set; }
        public string? FurNotice { get; set; } = null;
    }
}
