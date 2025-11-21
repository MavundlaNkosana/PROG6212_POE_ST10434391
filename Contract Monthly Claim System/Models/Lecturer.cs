using System;

namespace Contract_Monthly_Claim_System.Models
{
    // Added 'Settled' to track payment status
    public enum ClaimStatus { Draft, Submitted, UnderReview, Approved, Rejected, Settled }

    public class Lecturer
    {
        public Guid LecturerId { get; set; } = Guid.NewGuid();
        public string StaffNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
    }
}