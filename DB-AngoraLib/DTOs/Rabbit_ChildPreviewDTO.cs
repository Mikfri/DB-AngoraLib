﻿using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Rabbit_ChildPreviewDTO
    {
        public string EarCombId { get; init; }

        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; init; }

        public string? NickName { get; init; }
        public Models.Color Color { get; init; }
        public Gender Gender { get; init; }
        public string? OtherParentId{ get; init; }

        [DataType(DataType.ImageUrl)]
        public string? ProfilePicture { get; init; }
    }
}
