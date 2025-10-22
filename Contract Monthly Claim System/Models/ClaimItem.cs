using System;
using System.Collections.Generic;
using System.Linq;

namespace Contract_Monthly_Claim_System.Models
{
    public class ClaimItem
    {
        public Guid ClaimItemId { get; set; } = Guid.NewGuid();
        public Guid ClaimId { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public decimal HourlyRate { get; set; }
        public string ActivityDescription { get; set; }
        public decimal Amount => Math.Round(Hours * HourlyRate, 2);
    }
}
