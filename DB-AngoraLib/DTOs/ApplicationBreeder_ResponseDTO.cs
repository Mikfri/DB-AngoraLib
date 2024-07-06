using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record ApplicationBreeder_ResponseDTO
    {
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }
}
