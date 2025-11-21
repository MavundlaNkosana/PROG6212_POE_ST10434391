using System;

namespace Contract_Monthly_Claim_System.Models
{
    public class SupportingDocument
    {
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public Guid ClaimId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileUrl { get; set; } = string.Empty;
    }
}
