﻿using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Rabbit_ForbreedingPreviewDTO
    {
        public string EarCombId { get; init; }

        public string? NickName { get; init; }

        public string? OriginFullName { get; set; }   // set, så vi kan overskrive efter CopyProperties_FromAndTo
        public string? OwnerFullName { get; set; }    // set, så vi kan overskrive efter CopyProperties_FromAndTo

        public Race? Race { get; init; }
        public Color? Color { get; init; }

        public bool? ApprovedRaceColorCombination { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; init; }       

        public bool? IsJuvenile { get; init; }

        public Gender? Gender { get; init; }
        public IsPublic? ForSale { get; init; }
        public IsPublic? ForBreeding { get; init; }

        [DataType(DataType.PostalCode)]
        public int ZipCode { get; init; }

        public string City { get; init; }

        public string? FatherId_Placeholder { get; init; }
        public string? Father_EarCombId { get; init; }

        public string? MotherId_Placeholder { get; init; }
        public string? Mother_EarCombId { get; init; }

        [DataType(DataType.ImageUrl)]
        public string? ProfilePicture { get; init; }
    }
}
