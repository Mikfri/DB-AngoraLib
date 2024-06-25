using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class WeightMeasurement
    {
        public int Id { get; set; }
        public DateOnly DateMeasured { get; set; }
        public int DaysAfterBirth { get; set; }


        public int RabbitId { get; set; }
        public int WeightInGram { get; set; }
    }
}
