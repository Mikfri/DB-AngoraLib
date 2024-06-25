using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public enum BreederRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class BreederApplication         // TODO: Følg op på denne opsætning. Relation til User og BreederRole?
    {
        public int Id { get; set; }
        public DateOnly DateApplied { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string UserId { get; set; }
        public User UserApplicant { get; set; }

        public string RequestedBreederRegNo { get; set; }
        public string DocumentationPath { get; set; }
        public BreederRequestStatus Status { get; set; } = BreederRequestStatus.Pending;
        public string? RejectionReason { get; set; }

    }
}
