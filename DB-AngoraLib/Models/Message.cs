using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string SenderId { get; set; }
        public virtual User Sender { get; set; }

        public string RabbitId { get; set; }
        public virtual Rabbit Rabbit { get; set; }

        public string ReceiverId { get; set; }
        public virtual User Receiver { get; set; }


        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

    }
}
