using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record TransferRequest_CreateDTO
    {
        public string Rabbit_EarCombId { get; init; }
        public string Recipent_BreederRegNo { get; init; }

        public int? Price { get; init; }
        public string? SaleConditions { get; init; }
    }
}
