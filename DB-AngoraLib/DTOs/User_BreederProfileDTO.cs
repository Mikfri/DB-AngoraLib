using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record User_BreederProfileDTO
    {
        public string Id { get; init; }

        public string? BreederBrandLogo { get; init; }

        public string BreederBrandName { get; init; }
        public string BreederFullName { get; init; }
        public string BreederBrandDescription { get; init; }
    }
}
