using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record TransferRequest_SentFilterDTO
    {
        public RequestStatus? Status { get; init; }

        public string? Rabbit_EarCombId { get; init; }
        public string? Rabbit_NickName { get; init; }

        public string? Recipent_BreederRegNo { get; init; }
        public string? Recipent_FirstName { get; init; }

        public DateOnly? From_DateAccepted { get; init; }
    }
}
