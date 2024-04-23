using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
    }
}
