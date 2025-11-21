using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Contract_Monthly_Claim_System.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Contract_Monthly_Claim_System.Controllers
{
    public class ClaimsController : Controller
    {
        // In-memory database
        public static ConcurrentDictionary<Guid, Claim> Claims = new();
        public static ConcurrentDictionary<Guid, Lecturer> Lecturers = new();

        // Seed Data
        static ClaimsController()
        {
            var lecturer = new Lecturer
            {
                StaffNumber = "L001",
                FullName = "Dr. Thabo",
                Email = "thabo@uni.edu",
                HourlyRate = 550.00m // Updated rate for testing
            };
            Lecturers.TryAdd(lecturer.LecturerId, lecturer);

            var claim = new Claim
            {
                LecturerId = lecturer.LecturerId,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                Status = ClaimStatus.Draft
            };
            // Add initial item
            claim.Items.Add(new ClaimItem
            {
                Date = DateTime.Now.AddDays(-5),
                Hours = 2,
                HourlyRate = 550.00m,
                ActivityDescription = "Lecture Delivery"
            });

            Claims.TryAdd(claim.ClaimId, claim);
        }

        // GET: Claims/Index
        public IActionResult Index()
        {
            ViewBag.Lecturers = Lecturers;
            var claimsList = Claims.Values.OrderByDescending(c => c.Year).ThenByDescending(c => c.Month);
            return View(claimsList);
        }

        // GET: Claims/Create
        public IActionResult Create()
        {
            ViewBag.Lecturers = Lecturers.Values;
            return View();
        }

        // POST: Claims/Create
        [HttpPost]
        public IActionResult Create(Claim model)
        {
            model.Status = ClaimStatus.Draft;
            Claims[model.ClaimId] = model;
            return RedirectToAction("Edit", new { id = model.ClaimId });
        }

        // GET: Claims/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();

            if (Lecturers.TryGetValue(claim.LecturerId, out var lecturer))
            {
                ViewBag.LecturerName = lecturer.FullName;
                // CRITICAL: Passing the rate to the view for the JavaScript calculator
                ViewBag.HourlyRate = lecturer.HourlyRate;
            }
            else
            {
                ViewBag.LecturerName = "Unknown";
                ViewBag.HourlyRate = 0m;
            }

            return View(claim);
        }

        // POST: Claims/AddItem
        [HttpPost]
        public IActionResult AddItem(Guid claimId, DateTime date, decimal hours, string activityDescription)
        {
            if (!Claims.TryGetValue(claimId, out var claim)) return NotFound();

            if (claim.Status != ClaimStatus.Draft) return Unauthorized();

            // Server-Side Validation (Mirroring Client-Side)
            if (hours <= 0 || hours > 24)
            {
                TempData["ErrorMessage"] = "Invalid hours entered. Hours must be between 0.1 and 24.";
                return RedirectToAction("Edit", new { id = claimId });
            }

            // Get Rate from Server (Security: Don't trust client-side rate)
            var rate = 0m;
            if (Lecturers.TryGetValue(claim.LecturerId, out var lecturer))
            {
                rate = lecturer.HourlyRate;
            }

            claim.Items.Add(new ClaimItem
            {
                ClaimId = claimId,
                Date = date,
                Hours = hours,
                HourlyRate = rate,
                ActivityDescription = activityDescription
            });

            TempData["SuccessMessage"] = "Work item added successfully.";
            return RedirectToAction("Edit", new { id = claimId });
        }

        // POST: Claims/Submit
        [HttpPost]
        public IActionResult Submit(Guid id)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();

            // Business Rule Validation
            if (!claim.Items.Any())
            {
                TempData["ErrorMessage"] = "You cannot submit an empty claim. Please add work items.";
                return RedirectToAction("Edit", new { id = id });
            }

            claim.Status = ClaimStatus.Submitted;
            claim.SubmittedAt = DateTime.UtcNow;

            return RedirectToAction("Index");
        }

        // POST: Claims/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(Guid id, IFormFile file)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file.";
                return RedirectToAction("Edit", new { id });
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "File is too large (Max 5MB).";
                return RedirectToAction("Edit", new { id });
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".pdf" && ext != ".docx" && ext != ".xlsx")
            {
                TempData["ErrorMessage"] = "Only .pdf, .docx, and .xlsx allowed.";
                return RedirectToAction("Edit", new { id });
            }

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploads);

            var filename = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filepath = Path.Combine(uploads, filename);

            using (var fs = new FileStream(filepath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            claim.Documents.Add(new SupportingDocument
            {
                ClaimId = id,
                FileName = file.FileName,
                FileUrl = $"/uploads/{filename}",
                ContentType = file.ContentType,
                FileSize = file.Length
            });

            TempData["SuccessMessage"] = "Document uploaded!";
            return RedirectToAction("Edit", new { id });
        }
    }
}