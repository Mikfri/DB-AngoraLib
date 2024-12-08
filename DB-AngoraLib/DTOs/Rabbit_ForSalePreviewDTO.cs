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
    /// En DTO for Rabbit, som kun indeholder de mest nødvendige properties,
    /// for at en kunde kan danne overblik over en potentiel kanin til køb.
    /// </summary>
    public record Rabbit_ForSalePreviewDTO
    {
        public string EarCombId { get; init; }
        public string? NickName { get; init; }

        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; init; }

        public Race Race { get; set; }
        public Color Color { get; set; }
        public Gender Gender { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? ProfilePicture { get; init; }

        public string UserOwner { get; init; }
    }
}
