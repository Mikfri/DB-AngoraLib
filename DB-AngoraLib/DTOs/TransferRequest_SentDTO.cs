using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record TransferRequest_SentDTO
    {
        public int Id { get; init; }
        public RequestStatus Status { get; init; }

        public string? Rabbit_EarCombId { get; init; }
        public string? Rabbit_NickName { get; init; }

        public string? Recipent_BreederRegNo { get; init; }
        public string? Recipent_FirstName { get; init; }

        public int? Price { get; init; }
        public string? SaleConditions { get; init; }

        public DateOnly? DateAccepted { get; init; }
    }
}
