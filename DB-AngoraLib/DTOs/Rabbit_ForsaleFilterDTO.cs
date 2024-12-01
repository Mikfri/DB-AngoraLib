using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Rabbit_ForsaleFilterDTO
    {
        public string? RightEarId { get; init; }
        //public string? LeftEarId { get; init; } // Hvorfor sq en bruger kunne søge på dette??
        //public string? NickName { get; init; }
        public Race? Race { get; init; }
        public Color? Color { get; init; }
        public Gender? Gender { get; init; }
        public bool? IsJuvenile { get; init; }
        public bool? ApprovedRaceColorCombination { get; init; }
    }
}
