using Microsoft.AspNetCore.Mvc;
using Contract_Monthly_Claim_System.Models;
using Contract_Monthly_Claim_System.Services;
using System;

namespace Contract_Monthly_Claim_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsApiController : ControllerBase
    {
        private readonly ClaimVerificationService _verificationService;

        public ClaimsApiController()
        {
            _verificationService = new ClaimVerificationService();
        }

        // GET: api/ClaimsApi/5
        // Returns the JSON status of a claim including automated checks
        [HttpGet("{id}")]
        public ActionResult<object> GetClaimStatus(Guid id)
        {
            if (!ClaimsController.Claims.TryGetValue(id, out var claim))
            {
                return NotFound(new { Message = "Claim not found" });
            }

            // Run the automated verification logic on the fly
            var verificationResult = _verificationService.VerifyClaim(claim);

            return Ok(new
            {
                ClaimId = claim.ClaimId,
                LecturerId = claim.LecturerId,
                Period = $"{claim.Month}/{claim.Year}",
                Status = claim.Status.ToString(),
                TotalAmount = claim.TotalAmount,
                AutomatedChecks = new
                {
                    Passed = verificationResult.IsValid,
                    Errors = verificationResult.Errors,
                    Warnings = verificationResult.Warnings
                }
            });
        }
    }
}