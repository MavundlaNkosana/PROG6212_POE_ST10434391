using Microsoft.AspNetCore.Mvc;
using Contract_Monthly_Claim_System.Models;
using Contract_Monthly_Claim_System.Controllers; // To access shared data
using System;
using System.Linq;

namespace Contract_Monthly_Claim_System.Controllers
{
    public class HRController : Controller
    {
        // GET: HR Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // --- FEATURE 1: Lecturer Data Management ---

        public IActionResult ManageLecturers()
        {
            var lecturers = ClaimsController.Lecturers.Values.OrderBy(l => l.FullName);
            return View(lecturers);
        }

        public IActionResult EditLecturer(Guid id)
        {
            if (ClaimsController.Lecturers.TryGetValue(id, out var lecturer))
            {
                return View(lecturer);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult UpdateLecturer(Lecturer model)
        {
            if (ClaimsController.Lecturers.TryGetValue(model.LecturerId, out var existing))
            {
                // Update fields
                existing.FullName = model.FullName;
                existing.Email = model.Email;
                existing.StaffNumber = model.StaffNumber;
                existing.HourlyRate = model.HourlyRate;

                TempData["SuccessMessage"] = "Lecturer details updated successfully.";
                return RedirectToAction("ManageLecturers");
            }
            return NotFound();
        }

        // --- FEATURE 2: Reporting & Invoicing ---

        // Generates a report of all claims ready for payment (Status = Approved)
        public IActionResult PaymentReport()
        {
            var approvedClaims = ClaimsController.Claims.Values
                .Where(c => c.Status == ClaimStatus.Approved)
                .OrderByDescending(c => c.SubmittedAt)
                .ToList();

            return View(approvedClaims);
        }

        // Generates a printable invoice for a specific claim
        public IActionResult GenerateInvoice(Guid id)
        {
            if (!ClaimsController.Claims.TryGetValue(id, out var claim))
                return NotFound();

            if (!ClaimsController.Lecturers.TryGetValue(claim.LecturerId, out var lecturer))
                return NotFound();

            ViewBag.Lecturer = lecturer;
            ViewBag.GeneratedDate = DateTime.Now;

            // Simple Invoice Number logic: YEAR-MONTH-Last4Guid
            ViewBag.InvoiceNumber = $"INV-{claim.Year}{claim.Month:00}-{claim.ClaimId.ToString().Substring(0, 4).ToUpper()}";

            return View(claim);
        }
    }
}