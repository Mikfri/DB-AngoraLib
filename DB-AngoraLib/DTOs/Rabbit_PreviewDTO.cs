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
    /// for at en Breeder kan danne overblik over sine kaniner
    /// </summary>
    public record Rabbit_PreviewDTO
    {
        public string EarCombId { get; init; }
        public string RightEarId { get; init; }
        public string LeftEarId { get; init; }
        public string? NickName { get; init; }

        public Race Race { get; set; }
        public Color Color { get; set; }
        public Gender Gender { get; set; }
    }
}
