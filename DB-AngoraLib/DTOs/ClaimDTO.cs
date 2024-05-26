using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record ClaimDTO
    {
        public string Type { get; init; }
        public string Value { get; init; }
    }
}
