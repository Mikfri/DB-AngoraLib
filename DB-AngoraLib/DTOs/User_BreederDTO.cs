using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    /// <summary>
    /// USER, breeder DTO, med alle properties, inklusiv BreederRegNo (+Email og Phone fra IdentityUser)
    /// </summary>
    public record User_BreederDTO
    {
        public string BreederRegNo { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string RoadNameAndNo { get; init; }

        //[DataType(DataType.PostalCode)]
        public int ZipCode { get; init; }

        public string City { get; init; }

        //[DataType(DataType.EmailAddress)]
        public string Email { get; init; }

        //[DataType(DataType.PhoneNumber)]
        public string Phone { get; init; }
    }
}
