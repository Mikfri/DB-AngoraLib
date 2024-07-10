using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Login_ResponseDTO
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string RefreshToken { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
