using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public enum TransferStatus
    {
        Pending,
        Accepted,
        Rejected,
    }

    public class TransferRequst
    {
        public int Id { get; set; }

        public string? RabbitId { get; set; }
        public Rabbit? Rabbit { get; set; }

        public string? IssuerId { get; set; }
        public virtual User? UserIssuer { get; set; }

        public string? RecipentId { get; set; }
        public virtual User? UserRecipent { get; set; }

        public int? Price { get; set; }
        public string? SaleConditions { get; set; }

        public TransferStatus Status { get; set; } = TransferStatus.Pending;
        public DateOnly? DateAccepted { get; set; }

        //public bool IsDeleted { get; set; } = false;
    }

}
