using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected,
    }

    public class ApplicationBreeder
    {
        // Da navnet 'Id' følger EF Core namingkonvention, behøves der ikke at være en [Key] annotation,
        // eller angivning i DbContext OnModelCreating
        public int Id { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateOnly DateSubmitted { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string UserApplicantId { get; set; }
        public User UserApplicant { get; set; }

        public string RequestedBreederRegNo { get; set; }
        public string DocumentationPath { get; set; }

        public string? RejectionReason { get; set; }

        public ApplicationBreeder() { }
    }
}
