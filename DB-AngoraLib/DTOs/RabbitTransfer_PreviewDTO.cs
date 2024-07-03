using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record RabbitTransfer_PreviewDTO
    {
        public int Id { get; init; }

        public string RabbitId { get; init; }

        public string CurrentOwnerId { get; init; }

        public string ProposedOwnerId { get; init; }

        public RequestStatus Status { get; init; }

        public DateTime RequestDate { get; init; }
        public DateTime? ResponseDate { get; init; }

        public DateTime ExpirationDate { get; init; }
    }
}
