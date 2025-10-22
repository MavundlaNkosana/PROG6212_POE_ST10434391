using System;
using System.Collections.Generic;
using System.Linq;

namespace Contract_Monthly_Claim_System.Models
{
    public enum ClaimStatus { Draft, Submitted, UnderReview, Approved, Rejected }

    public class Lecturer
    {
        public Guid LecturerId { get; set; } = Guid.NewGuid();
        public string StaffNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal HourlyRate { get; set; }
    }
}
