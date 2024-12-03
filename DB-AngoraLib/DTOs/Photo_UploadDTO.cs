using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Photo_UploadDTO
    {
        public string File { get; set; }
        public int? RabbitId { get; set; }
        public int? UserId { get; set; }
    }
}
