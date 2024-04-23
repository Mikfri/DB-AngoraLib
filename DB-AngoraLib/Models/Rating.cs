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
        public int Id { get; set; }

        public string RightEarId{ get; set; }
        public string LeftEarId{ get; set; }
        public Rabbit Rabbit { get; set; }

        public DateOnly DateRated { get; set; }
        public int WeightPoint { get; set; }
        public string? WeightNotice { get; set; } = null;
        public int BodyPoint { get; set; }
        public string? BodyNotice { get; set; }
        public int FurPoint { get; set; }
        public string? FurNotice { get; set; }

        public int? TotalPoint { get; set; }
    }
}
