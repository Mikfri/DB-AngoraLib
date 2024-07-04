using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record RabbitTransfer_UpdateDTO
    {
        //public string CurrentOwner_BreederRegNo { get; init; }
        //public string EarCombId { get; init; }
        public int? Price { get; init; }
        public string? SaleConditions { get; init; }

        //public string ProposedOwner_BreederRegNo { get; init; }
        //public DateTime? DateAccepted { get; init; }
    }
}
