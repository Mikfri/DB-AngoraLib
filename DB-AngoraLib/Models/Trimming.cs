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
        public int Id { get; set; }

        public int RabbitId { get; set; }
        public Rabbit Rabbit { get; set; }

        public DateOnly DateTrimmed { get; set; }
        public int DaysSinceLastTrim
        {
            get      //Derrived property
            {
                DateTime dateNow = DateTime.Now;
                DateTime dateTrimmed = new DateTime(DateTrimmed.Year, DateTrimmed.Month, DateTrimmed.Day);

                return (int)(dateNow - dateTrimmed).TotalDays;
            }
        }

        public int FirstSortmentWeightGram { get; set; }

        public int SecondSortmentWeightGram { get; set; }

        public int DisposableWoolWeightGram { get; set; }

        public int? TimeUsedMinutes { get; set; }

        public float? HairLengthCm { get; set; }

        public float? WoolDensity { get; set; }

    }
}
