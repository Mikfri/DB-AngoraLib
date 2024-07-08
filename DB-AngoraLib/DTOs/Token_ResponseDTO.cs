using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Token_ResponseDTO
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
    }
}
