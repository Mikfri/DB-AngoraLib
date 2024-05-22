using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    /// <summary>
    /// DTO for response; string UserName, bool IsSuccessful, IEnumerable<string> Errors
    /// </summary>
    public record Register_ResponseDTO
    {
        public string UserName { get; set; }
        public bool IsSuccessful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
