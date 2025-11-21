using System.Collections.Generic;

namespace Contract_Monthly_Claim_System.Models
{
    public class VerificationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();

        // Helper to check if the claim is "perfect"
        public bool IsPerfect => Errors.Count == 0 && Warnings.Count == 0;
    }
}