using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record UserLoginDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; init; }
    }
}
