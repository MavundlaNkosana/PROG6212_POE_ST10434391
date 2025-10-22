namespace Contract_Monthly_Claim_System.Helpers
{
    public static class ClaimStatusHelper
    {
        public static string GetStatusBadgeClass(Models.ClaimStatus status)
        {
            return status switch
            {
                Models.ClaimStatus.Draft => "badge-secondary",
                Models.ClaimStatus.Submitted => "badge-primary",
                Models.ClaimStatus.UnderReview => "badge-info",
                Models.ClaimStatus.Approved => "badge-success",
                Models.ClaimStatus.Rejected => "badge-danger",
                _ => "badge-light"
            };
        }
    }
}
