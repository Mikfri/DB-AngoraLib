using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Vaccination
    {
        public int Id { get; set; }
        public string VaccineName { get; set; }
        public string Description { get; set; }

        public bool Pill { get; set; }
        public bool Injection { get; set; }

        public DateOnly Date { get; set; }

        public string RabbitId { get; set; }
        public Rabbit Rabbit { get; set; }
    }
}
