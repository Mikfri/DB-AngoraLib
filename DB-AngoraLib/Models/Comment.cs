using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateOnly DateCommented { get; set; }
        public string Description { get; set; }

        public Comment(DateOnly dateCommented, string description)
        {
            DateCommented = dateCommented;
            Description = description;
        }

        public Comment() { }



    }
}
