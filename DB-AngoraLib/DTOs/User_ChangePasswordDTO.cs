using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    /// <summary>
    /// string UserId, string CurrentPassword, string NewPassword
    /// </summary>
    public record User_ChangePasswordDTO
    {
        public string UserId { get; set; }

        //[DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        //[DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
