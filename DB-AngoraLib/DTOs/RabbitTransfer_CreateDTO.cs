using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record RabbitTransfer_CreateDTO
    {
        public string EarCombId { get; init; }
        public string ProposedOwnerBreederRegNo { get; init; }
    }
}
