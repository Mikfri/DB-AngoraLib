using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record User_ProfileBreederBrandDTO
    {
        public string Id { get; init; }

        [DataType(DataType.ImageUrl)]
        public string? BreederBrandLogo { get; init; }

        public string BreederBrandName { get; init; }
        
        //-------------------- User properties
        public string Breeder_FullName { get; init; }
        public string Breeder_RoadNameAndNo { get; init; }
        public string Breeder_City { get; init; }

        [DataType(DataType.PostalCode)]
        public int Breeder_ZipCode { get; init; }

        [DataType(DataType.PhoneNumber)]
        public string Breeder_Phone { get; init; }

        [DataType(DataType.Text)]
        public string BreederBrandDescription { get; init; }

        public List<Rabbit_PreviewDTO> RabbitsForSaleOrBreeding { get; init; } = new List<Rabbit_PreviewDTO>();
    }
}
