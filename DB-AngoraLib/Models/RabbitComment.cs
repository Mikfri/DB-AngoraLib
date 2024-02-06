using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class RabbitComment
    {
        public int Id { get; set; }
        public DateOnly CommentDate { get; set; }
        public string Description { get; set; }


    }
}
