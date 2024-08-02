using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Rabbit_UpdateDTO
    {
        //public string RightEarId { get; init; }   // Nope! Det skal ikke være muligt at ændre øre-mærker!
        //public string LeftEarId { get; init; }    
        //public string? OwnerId { get; init; }     // En seperat DTO bør nok laves for ejerskifte 

        public string? NickName { get; init; }
        public Race? Race { get; init; }
        public Color? Color { get; init; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; init; }
        [DataType(DataType.Date)]
        public DateOnly? DateOfDeath { get; init; }

        public Gender? Gender { get; init; }
        public IsPublic? ForSale { get; init; }
        public IsPublic? ForBreeding { get; init; }

        public string? FatherId_Placeholder { get; init; }
        public string? MotherId_Placeholder { get; init; }
    }
}
