using Microsoft.AspNetCore.Mvc;
using Contract_Monthly_Claim_System.Models;
using System;
using System.Linq;

namespace Contract_Monthly_Claim_System.Controllers
{
    public class CoordinatorController : Controller
    {
        public IActionResult Index()
        {
            var pendingClaims = ClaimsController.Claims.Values
                .Where(c => c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.UnderReview)
                .OrderByDescending(c => c.SubmittedAt);
            return View(pendingClaims);
        }

        public IActionResult Details(Guid id)
        {
            if (ClaimsController.Claims.TryGetValue(id, out var claim))
            {
                return View(claim);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Approve(Guid id, string comments)
        {
            if (ClaimsController.Claims.TryGetValue(id, out var claim))
            {
                claim.Status = ClaimStatus.Approved;
                claim.Approvals.Add(new Approval
                {
                    ApproverRole = "Coordinator", // Role can be dynamic in a real app
                    DecisionDate = DateTime.UtcNow,
                    IsApproved = true,
                    Comments = comments
                });
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Reject(Guid id, string comments)
        {
            if (ClaimsController.Claims.TryGetValue(id, out var claim))
            {
                claim.Status = ClaimStatus.Rejected;
                claim.Approvals.Add(new Approval
                {
                    ApproverRole = "Coordinator",
                    DecisionDate = DateTime.UtcNow,
                    IsApproved = false,
                    Comments = comments
                });
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}

