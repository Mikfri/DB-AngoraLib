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
    /// Indeholder alle properties for en Rabbit, som en User skal kunne se
    /// </summary>
    public record Rabbit_ProfileDTO
    {
        public string EarCombId { get; init; }

        //public string? RightEarId { get; init; }
        //public string? LeftEarId { get; init; } 

        public string? NickName { get; init; }

        public string? OriginStatusMessage { get; init; }
        //public string? OriginId { get; init; }      // HACK: Skal ikke ud til brugerne, da det er ulæseligt. Overvej en string message
        public string? OwnerId { get; init; }

        public Race? Race { get; init; }
        public Color? Color { get; init; }

        public bool? ApprovedRaceColorCombination { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; init; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfDeath { get; init; }

        public bool? IsJuvenile { get; init; }

        public Gender? Gender { get; init; }
        public ForSale? ForSale { get; init; }

        public string? FatherId_Placeholder { get; init; }
        public string? FatherStatusMessage { get; init; }

        public string? MotherId_Placeholder { get; init; }
        public string? MotherStatusMessage { get; init; }

        //public Rabbit_PedigreeDTO Pedigree { get; init; }

        public List<Rabbit_ChildPreviewDTO?> Children { get; init; }


    }
}
