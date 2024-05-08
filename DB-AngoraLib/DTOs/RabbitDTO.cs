using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    /// <summary>
    /// Basis DTO for Rabbit, med alle dens properties
    /// </summary>
    public record RabbitDTO
    {
        public string RightEarId { get; init; }
        public string LeftEarId { get; init; }

        //public string? OwnerId { get; init; }

        public string? NickName { get; init; }
        public Race Race { get; init; }
        public Color Color { get; init; }
        //public bool? ApprovedRaceColorCombination { get; init; }
        public DateOnly DateOfBirth { get; init; }
        public DateOnly? DateOfDeath { get; init; }
        //public bool IsJuvenile { get; init; }
        public Gender Gender { get; init; }
        public IsPublic? IsPublic { get; init; }
    }
}
