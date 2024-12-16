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
    /// Indeholder alle properties som en kanin avler er interesseret i at se.
    /// </summary>
    public record Rabbit_ProfileDTO
    {
        public string EarCombId { get; init; }

        //public string? RightEarId { get; init; }
        //public string? LeftEarId { get; init; } 

        public string? NickName { get; init; }

        public string? OriginFullName { get; set; }   // set, så vi kan overskrive efter CopyProperties_FromAndTo
        public string? OwnerFullName { get; set; }    // set, så vi kan overskrive efter CopyProperties_FromAndTo

        public Race? Race { get; init; }
        public Color? Color { get; init; }

        public bool? ApprovedRaceColorCombination { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; init; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfDeath { get; init; }

        public bool? IsJuvenile { get; init; }

        public Gender? Gender { get; init; }
        public IsPublic? ForSale { get; init; }
        public IsPublic? ForBreeding { get; init; }

        public string? FatherId_Placeholder { get; init; }
        public string? Father_EarCombId { get; init; }

        public string? MotherId_Placeholder { get; init; }
        public string? Mother_EarCombId { get; init; }

        [DataType(DataType.ImageUrl)]
        public string? ProfilePicture { get; init; }

        //public Rabbit_PedigreeDTO Pedigree { get; init; }
        public List<Photo_DTO> ?Photos { get; init; }
        public List<Rabbit_ChildPreviewDTO> ?Children { get; init; }
    }
}
