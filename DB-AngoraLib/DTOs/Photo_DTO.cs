using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Photo_DTO
    {
        public int Id { get; set; }
        public string FilePath { get; set; } // or URI if using cloud storage
        public string FileName { get; set; }   
        public DateTime UploadDate { get; set; }

        public string? RabbitId { get; set; }
        public string? UserId { get; set; }
        public bool IsProfilePicture { get; set; }
    }
}
