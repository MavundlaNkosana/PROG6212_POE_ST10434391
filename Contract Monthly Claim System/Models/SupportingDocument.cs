using System;
using System.Collections.Generic;
using System.Linq;

namespace Contract_Monthly_Claim_System.Models
{
    public class SupportingDocument
    {
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public Guid ClaimId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string FileUrl { get; set; }
    }
}
