using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Contract_Monthly_Claim_System.Controllers
{
    public class CoordinatorController : Controller
    {
        // GET: Coordinator/Index
        // This is the main dashboard for approvers. It shows a list of all claims that need attention.
        public IActionResult Index()
        {
            var pendingClaims = ClaimsController.Claims.Values
                .Where(c => c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.UnderReview)
                .OrderBy(c => c.SubmittedAt)
                .ToList();

            ViewBag.Lecturers = ClaimsController.Lecturers;
            return View(pendingClaims);
        }

        // GET: Coordinator/Details/5
        // This view provides an in-depth look at a specific claim for verification.
        public IActionResult Details(Guid id)
        {
            if (!ClaimsController.Claims.TryGetValue(id, out var claim))
            {
                return NotFound();
            }

            var lecturer = ClaimsController.Lecturers[claim.LecturerId];
            ViewBag.LecturerName = lecturer.FullName;

            return View(claim);
        }

        // POST: Coordinator/Approve
        [HttpPost]
        public IActionResult Approve(Guid id, string comments)
        {
            if (ClaimsController.Claims.TryGetValue(id, out var claim))
            {
                claim.Status = ClaimStatus.Approved;
                claim.Approvals.Add(new Approval
                {
                    ApproverRole = "Coordinator", // This would be dynamic in a real app
                    DecisionDate = DateTime.UtcNow,
                    IsApproved = true,
                    Comments = comments
                });
            }
            return RedirectToAction("Index");
        }

        // POST: Coordinator/Reject
        [HttpPost]
        public IActionResult Reject(Guid id, string comments)
        {
            if (ClaimsController.Claims.TryGetValue(id, out var claim))
            {
                // In a real system, you might set this to a "NeedsRevision" status
                // to send it back to the lecturer.
                claim.Status = ClaimStatus.Rejected;
                claim.Approvals.Add(new Approval
                {
                    ApproverRole = "Coordinator",
                    DecisionDate = DateTime.UtcNow,
                    IsApproved = false,
                    Comments = comments
                });
            }
            return RedirectToAction("Index");
        }
    }
}

