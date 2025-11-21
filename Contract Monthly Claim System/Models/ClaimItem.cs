using System;

namespace Contract_Monthly_Claim_System.Models
{
    public class ClaimItem
    {
        public Guid ClaimItemId { get; set; } = Guid.NewGuid();
        public Guid ClaimId { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public decimal HourlyRate { get; set; }
        public string ActivityDescription { get; set; } = string.Empty;
        public decimal Amount => Math.Round(Hours * HourlyRate, 2);
    }
}
