using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace Contract_Monthly_Claim_System.Controllers
{
    public class ClaimsController : Controller
    {
        // In-memory stores for prototype
        public static ConcurrentDictionary<Guid, Claim> Claims = new();
        public static ConcurrentDictionary<Guid, Lecturer> Lecturers = new();

        // Seed a lecturer and a draft claim for demonstration
        static ClaimsController()
        {
            var lecturer = new Lecturer { StaffNumber = "L001", FullName = "Dr. Thabo", Email = "thabo@uni.edu", HourlyRate = 500.00m };
            Lecturers.TryAdd(lecturer.LecturerId, lecturer);

            var draftClaim = new Claim { LecturerId = lecturer.LecturerId, Month = 10, Year = 2025, Status = ClaimStatus.Draft };
            draftClaim.Items.Add(new ClaimItem { Date = new DateTime(2025, 10, 5), Hours = 3, HourlyRate = 500, ActivityDescription = "Grading assignments" });
            Claims.TryAdd(draftClaim.ClaimId, draftClaim);
        }

        public IActionResult Index()
        {
            return View(Claims.Values.OrderByDescending(c => c.Year).ThenByDescending(c => c.Month));
        }

        public IActionResult Create()
        {
            ViewBag.Lecturers = Lecturers.Values;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Claim model)
        {
            model.Status = ClaimStatus.Draft;
            Claims[model.ClaimId] = model;
            return RedirectToAction("Edit", new { id = model.ClaimId });
        }

        public IActionResult Edit(Guid id)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();

            var lecturer = Lecturers[claim.LecturerId];
            ViewBag.LecturerName = lecturer.FullName;
            ViewBag.HourlyRate = lecturer.HourlyRate; // Pass rate to the view

            return View(claim);
        }

        // NEW: Action to handle adding a single item to the claim
        [HttpPost]
        public IActionResult AddItem(Guid claimId, DateTime date, decimal hours, string activityDescription)
        {
            if (!Claims.TryGetValue(claimId, out var claim)) return NotFound();
            if (claim.Status != ClaimStatus.Draft) return Unauthorized(); // Can't add items to a submitted claim

            var lecturer = Lecturers[claim.LecturerId];
            var newItem = new ClaimItem
            {
                ClaimId = claimId,
                Date = date,
                Hours = hours,
                HourlyRate = lecturer.HourlyRate, // Use lecturer's default rate
                ActivityDescription = activityDescription
            };
            claim.Items.Add(newItem);

            return RedirectToAction("Edit", new { id = claimId });
        }

        [HttpPost]
        public IActionResult Submit(Guid id)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();
            claim.Status = ClaimStatus.Submitted;
            claim.SubmittedAt = DateTime.UtcNow;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Guid id, IFormFile file)
        {
            // This functionality is preserved for a later step
            return RedirectToAction("Edit", new { id });
        }
    }
}

