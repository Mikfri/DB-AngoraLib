using MimeKit.Cryptography;
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
        //-----------------------------: INSTANCE FIELDS
        private DateTime requestDate;


        //-----------------------------: PROPERTIES

        public int Id { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateOnly DateAccepted { get; set; }


        public string CurrentOwnerId { get; set; }
        public virtual User UserCurrentOwner { get; set; }

        public string RabbitId { get; set; }
        public Rabbit RabbitInTrans { get; set; }

        public int? Price { get; set; }
        public string? SaleConditions { get; set; }

        public string ProposedOwnerId { get; set; }
        public virtual User UserProposedOwner { get; set; }


        //public bool IsDeleted { get; set; } = false;
    }

}
