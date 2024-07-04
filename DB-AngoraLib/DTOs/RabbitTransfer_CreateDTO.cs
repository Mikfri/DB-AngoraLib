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
        public string ProposedOwner_BreederRegNo { get; init; }
        public int? Price { get; init; }
        public string? SaleConditions { get; init; }
    }
}
