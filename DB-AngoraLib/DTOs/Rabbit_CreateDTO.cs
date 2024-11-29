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
    /// Basis DTO for Rabbit, med alle dens properties
    /// </summary>
    public record Rabbit_CreateDTO
    {
        [Display(Name = "Højre øre")]
        //[RegularExpression(@"^\d{4}$", ErrorMessage = "Kanin.HøjreØreId: SKAL bestå af 4 tal!")]
        public string RightEarId { get; init; }

        [Display(Name = "Venstre øre")]
        [RegularExpression(@"^\d{3,5}$", ErrorMessage = "Kanin.VenstreØreId: Skal være imellem 3 og 5 numeriske tal")]
        [Required(ErrorMessage = "Kanin.Id: Feldtet skal udfyldes")]
        public string LeftEarId { get; init; }
        //public string? OwnerId { get; init; }

        public string? NickName { get; init; }

        [Required(ErrorMessage = "Kanin.Race: Vælg en gyldig race fra listen")]
        public Race Race { get; init; }

        [Required(ErrorMessage = "Ugyldig farve! Vælg fra listen")]
        public Color Color { get; init; }        //public bool? ApprovedRaceColorCombination { get; init; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Kanin.Fødselsdato: Feldtet skal udfyldes")]
        public DateOnly DateOfBirth { get; init; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfDeath { get; init; }

        //public bool IsJuvenile { get; init; }
        [Required(ErrorMessage = "Kanin.Køn: Vælg et gyldigt køn (Buck/Doe)")]
        public Gender Gender { get; init; }

        [Required(ErrorMessage = "Kanin.TilSalg: Dette feldt er påkrævet")]
        public IsPublic ForSale { get; init; }

        [Required(ErrorMessage = "Kanin.TilParring: Dette feldt er påkrævet")]
        public IsPublic ForBreeding { get; init; }

        //[RegularExpression(@"^\d{4}-\d{3,5}$", ErrorMessage = "Kanin.ForældreId: Skal være i formatet RightEarId-LeftEarId (f.eks. 5095-021)")]
        public string? FatherId_Placeholder{ get; init; }

        //[RegularExpression(@"^\d{4}-\d{3,5}$", ErrorMessage = "Kanin.ForældreId: Skal være i formatet RightEarId-LeftEarId (f.eks. 5095-021)")]
        public string? MotherId_Placeholder { get; init; }
    }
}
