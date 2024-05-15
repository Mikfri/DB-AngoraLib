using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    /// <summary>
    /// USER, basis DTO, for dem der IKKE har breeder role (+Email og Phone fra IdentityUser)
    /// </summary>
    public record User_CreateBasicDTO
    {
        public string Email { get; init; }
        public string Password { get; init; }

        //[DataType(DataType.PhoneNumber)]
        public string Phone { get; init; }

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string RoadNameAndNo { get; init; }

        //[DataType(DataType.PostalCode)]
        public int ZipCode { get; init; }

        public string City { get; init; }

        

        

    }
}
