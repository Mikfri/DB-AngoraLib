using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record ApplicationBreeder_PreviewDTO
    {
        public int  Id { get; init; }
        public RequestStatus Status { get; init; }
        public DateOnly DateSubmitted { get; init; }
        public string UserApplicant_FullName { get; init; }
        public string UserApplicant_RequestedBreederRegNo { get; init; }
        public string UserApplicant_DocumentationPath { get; init; }

        public string? RejectionReason { get; init; }
    }
}
