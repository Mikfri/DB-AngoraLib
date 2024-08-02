using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Wool
    {
        public int Id { get; set; }

        public string WoolType { get; set; }
        public string WoolColor { get; set; }
        public string WoolQuality { get; set; }
        public double WoolLength { get; set; }
        public double WoolWeight { get; set; }
        public string? WoolDescription { get; set; }

        public Wool() { }
    }
}
