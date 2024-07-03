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
        public int Generation { get; init; }

        public string Relation { get; set; }

        public string EarCombId { get; init; }
        public string? NickName { get; init; }
        public DateOnly DateOfBirth { get; init; }
        public Race Race { get; init; }
        public Color Color { get; init; }
        public string? UserOriginName { get; init; }
        public string? UserOwnerName { get; init; }
        public Rabbit_PedigreeDTO? Father { get; init; }
        public Rabbit_PedigreeDTO? Mother { get; init; }
    }

}
