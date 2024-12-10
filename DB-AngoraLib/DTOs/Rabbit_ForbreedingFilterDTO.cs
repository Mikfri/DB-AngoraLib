using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Rabbit_ForbreedingFilterDTO
    {
        public string? RightEarId { get; init; }
        //public string? LeftEarId { get; init; } // Hvorfor sq en bruger kunne søge på dette??
        //public string? NickName { get; init; }
        [DataType(DataType.Date)]
        public DateOnly? BornAfter { get; init; }

        [DataType(DataType.PostalCode)]
        [Range(1000, 9999, ErrorMessage = "Indtast et gyldigt dansk postnummer mellem 1000 og 9999.")]
        public int? MinZipCode { get; init; }

        [DataType(DataType.PostalCode)]
        [Range(1000, 9999, ErrorMessage = "Indtast et gyldigt dansk postnummer mellem 1000 og 9999.")]
        public int? MaxZipCode { get; init; }

        public Race? Race { get; init; }
        public Color? Color { get; init; }
        public Gender? Gender { get; init; }
        //public bool? IsJuvenile { get; init; }
        public bool? ApprovedRaceColorCombination { get; init; }
    }
}
