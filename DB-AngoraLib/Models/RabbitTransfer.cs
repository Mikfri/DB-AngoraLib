using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class RabbitTransfer
    {
        public int Id { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime RequestDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public string CurrentOwnerId { get; set; }
        public User UserCurrentOwner { get; set; }

        public string RabbitId { get; set; }
        public Rabbit RabbitInTrans { get; set; }

        public string ProposedOwnerId { get; set; }
        public User UserProposedOwner { get; set; }
 
        public DateTime? ResponseDate { get; set; }
        //public bool IsDeleted { get; set; } = false;

    }
}
