using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    //public record Application_BreederDTO(string UserId, string RequestedBreederRegNo, string DocumentationPath);
    public record Application_BreederDTO
    {
        //public string UserId { get; init; }
        public string RequestedBreederRegNo { get; init; }
        public string DocumentationPath { get; init; }
    }
}
