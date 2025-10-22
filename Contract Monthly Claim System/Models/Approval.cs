using System;
using System.Collections.Generic;
using System.Linq;

namespace Contract_Monthly_Claim_System.Models
{
    public class Approval
    {
        public Guid ApprovalId { get; set; } = Guid.NewGuid();
        public Guid ClaimId { get; set; }
        public Guid ApproverId { get; set; }
        public string ApproverRole { get; set; } // Coordinator/Manager
        public DateTime DecisionDate { get; set; }
        public bool IsApproved { get; set; }
        public string Comments { get; set; }
    }
}
