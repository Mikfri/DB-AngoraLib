using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public enum FavoriteType
    {
        Rabbit,
        Wool
    }

    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string ItemId { get; set; } // This could be RabbitId or WoolId
        public FavoriteType ItemType { get; set; } // "Rabbit" or "Wool"
    }
}
