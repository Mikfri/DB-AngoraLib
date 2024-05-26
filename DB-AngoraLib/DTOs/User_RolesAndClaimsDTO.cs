using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record User_RolesAndClaimsDTO
    {
        public string UserId { get; init; }
        public string UserName { get; init; }
        public List<string> Roles { get; init; }
        public List<ClaimDTO> Claims { get; init; }
    }
}
