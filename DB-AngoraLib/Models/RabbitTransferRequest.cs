using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class RabbitTransferRequest
    {
        public int Id { get; set; }
        public string EarCombId { get; set; }
        public string CurrentOwnerId { get; set; }
        public string NewOwnerId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
