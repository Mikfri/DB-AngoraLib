using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    /// <summary>
    /// Basis DTO for User, med alle dens properties + Email og Phone fra IdentityUser
    /// </summary>
    public record UserDTO
    {
        public string BreederRegNo { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string RoadNameAndNo { get; init; }
        public int ZipCode { get; init; }
        public string City { get; init; }
        public string Email { get; init; }
        public string Phone { get; init; }
    }
}
