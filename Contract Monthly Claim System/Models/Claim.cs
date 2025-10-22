using System;
using System.Collections.Generic;
using System.Linq;

namespace Contract_Monthly_Claim_System.Models
{
    public class Claim
    {
        public Guid ClaimId { get; set; } = Guid.NewGuid();
        public Guid LecturerId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.Draft;
        public List<ClaimItem> Items { get; set; } = new List<ClaimItem>();
        public List<SupportingDocument> Documents { get; set; } = new List<SupportingDocument>();
        public string Notes { get; set; }

        public decimal TotalHours => Items.Sum(i => i.Hours);
        public decimal TotalAmount => Items.Sum(i => i.Amount);
    }
}
