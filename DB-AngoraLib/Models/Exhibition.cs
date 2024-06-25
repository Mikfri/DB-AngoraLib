using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Exhibition
    {
        public int Id { get; set; }
        public DateOnly ExhibitionDate { get; set; }
        public string Name { get; set; }
    }
}
