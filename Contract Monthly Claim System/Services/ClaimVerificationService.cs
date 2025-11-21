using Contract_Monthly_Claim_System.Controllers;
using Contract_Monthly_Claim_System.Models;
using System.Linq;

namespace Contract_Monthly_Claim_System.Services
{
    public class ClaimVerificationService
    {
        // Define policies constants
        private const decimal MAX_HOURS_PER_MONTH = 160;
        private const decimal MAX_CLAIM_AMOUNT = 50000; // Currency limit

        public VerificationResult VerifyClaim(Claim claim)
        {
            var result = new VerificationResult { IsValid = true };

            // 1. Check for existence of Lecturer
            if (!ClaimsController.Lecturers.TryGetValue(claim.LecturerId, out var lecturer))
            {
                result.Errors.Add("CRITICAL: Lecturer profile not found in database.");
                result.IsValid = false;
                return result;
            }

            // 2. Rule: Policy - Max Hours Check
            if (claim.TotalHours > MAX_HOURS_PER_MONTH)
            {
                result.Warnings.Add($"POLICY ALERT: Total hours ({claim.TotalHours}) exceeds standard monthly limit of {MAX_HOURS_PER_MONTH}.");
            }

            // 3. Rule: Data Integrity - Rate Verification
            // Detect if the claim item rate differs from the lecturer's contract rate
            foreach (var item in claim.Items)
            {
                if (item.HourlyRate != lecturer.HourlyRate)
                {
                    result.Errors.Add($"DATA MISMATCH: Item on {item.Date.ToShortDateString()} has rate {item.HourlyRate:C} but contract rate is {lecturer.HourlyRate:C}.");
                    result.IsValid = false;
                }
            }

            // 4. Rule: Supporting Documents
            if (!claim.Documents.Any())
            {
                result.Warnings.Add("COMPLIANCE: No supporting documents attached. Manual verification required.");
            }

            // 5. Rule: Max Amount Cap
            if (claim.TotalAmount > MAX_CLAIM_AMOUNT)
            {
                result.Warnings.Add($"FINANCE: Total amount {claim.TotalAmount:C} requires simplistic Manager approval (Exceeds {MAX_CLAIM_AMOUNT:C}).");
            }

            return result;
        }
    }
}