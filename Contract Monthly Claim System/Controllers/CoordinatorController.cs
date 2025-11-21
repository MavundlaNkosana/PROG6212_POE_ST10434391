using Microsoft.AspNetCore.Mvc;
using Contract_Monthly_Claim_System.Models;
using Contract_Monthly_Claim_System.Services;
using System;
using System.Linq;

namespace Contract_Monthly_Claim_System.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ClaimVerificationService _verificationService;

        public CoordinatorController()
        {
            _verificationService = new ClaimVerificationService();
        }

        public IActionResult Index()
        {
            // FIX: Pass the Lecturers dictionary to the view so names can be looked up
            ViewBag.Lecturers = ClaimsController.Lecturers;

            var pendingClaims = ClaimsController.Claims.Values
                .Where(c => c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.UnderReview)
                .OrderByDescending(c => c.SubmittedAt);

            return View(pendingClaims);
        }

        public IActionResult Details(Guid id)
        {
            if (ClaimsController.Claims.TryGetValue(id, out var claim))
            {
                var verificationResult = _verificationService.VerifyClaim(claim);

                ViewBag.VerificationResult = verificationResult;

                if (ClaimsController.Lecturers.TryGetValue(claim.LecturerId, out var lecturer))
                {
                    ViewBag.LecturerName = lecturer.FullName;
                    ViewBag.ContractRate = lecturer.HourlyRate;
                }

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
                    ApproverRole = "Coordinator",
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