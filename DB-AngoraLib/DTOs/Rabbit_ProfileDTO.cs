using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    /// <summary>
    /// Indeholder alle properties for en Rabbit, som en User skal kunne se og søge på.
    /// </summary>
    public record Rabbit_ProfileDTO
    {
        public string RightEarId { get; init; }   
        public string LeftEarId { get; init; }    
        
        public string? OwnerId { get; init; }

        public string? NickName { get; init; }
        public Race? Race { get; init; }
        public Color? Color { get; init; }

        public bool? ApprovedRaceColorCombination { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; init; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfDeath { get; init; }

        public bool IsJuvenile { get; init; }

        public Gender? Gender { get; init; }
        public OpenProfile? OpenProfile { get; init; }
    }
}
