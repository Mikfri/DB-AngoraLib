using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Rabbit_PedigreeDTO
    {
        public Rabbit Rabbit { get; init; }
        public Rabbit_PedigreeDTO Father { get; init; }
        public Rabbit_PedigreeDTO Mother { get; init; }
        public int Generation { get; init; }
    }
}
