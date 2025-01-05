using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class SaleDetails
    {
        public int Id { get; set; }

        public decimal Price { get; set; }
        public DateOnly DatePublished { get; set; }

        public string SaleDescription { get; set; }

        public string? RabbitId { get; set; }
        public Rabbit? Rabbit { get; set; }

        public int? WoolId { get; set; }
        public Wool? Wool { get; set; }

        // Validerer at kun en af de to fremmednøgler er sat
        [NotMapped]
        public bool IsValid => (RabbitId != null) ^ (WoolId != null);
    }
}
