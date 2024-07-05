using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record TransferRequest_ContractDTO
    {
        public int Id { get; init; }
        public RequestStatus Status { get; init; }
        public DateOnly? DateAccepted { get; init; }
        //-------------: Issuer
        public string Issuer_BreederRegNo { get; init; }
        public string Issuer_FullName { get; init; }
        public string Issuer_Email { get; init; }
        public string Issuer_RoadNameAndNo { get; init; }
        public int Issuer_ZipCode { get; init; }
        public string Issuer_City { get; init; }
        //-------------: Recipent
        public string Recipent_BreederRegNo { get; init; }
        public string Recipent_FullName { get; init; }
        public string Recipent_Email { get; init; }
        public string Recipent_RoadNameAndNo { get; init; }
        public int Recipent_ZipCode { get; init; }
        public string Recipent_City { get; init; }
        //-------------: Rabbit
        public string EarCombId { get; init; }
        public string? NickName { get; init; }
        public int? Price { get; init; }
        public string? SaleConditions { get; init; }

       

    }
}
