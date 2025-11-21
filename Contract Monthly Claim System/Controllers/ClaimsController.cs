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
        // We make these public static so they act as a shared "database" for the app
        public static ConcurrentDictionary<Guid, Claim> Claims = new();
        public static ConcurrentDictionary<Guid, Lecturer> Lecturers = new();

        // Constructor to seed data
        static ClaimsController()
        {
            // 1. Create a demo Lecturer
            var lecturer = new Lecturer
            {
                StaffNumber = "L001",
                FullName = "Dr. Thabo",
                Email = "thabo@uni.edu",
                HourlyRate = 500.00m
            };
            Lecturers.TryAdd(lecturer.LecturerId, lecturer);

            // 2. Create a demo Claim
            var claim = new Claim
            {
                LecturerId = lecturer.LecturerId,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                Status = ClaimStatus.Draft
            };
            // Add an item to the claim
            claim.Items.Add(new ClaimItem
            {
                Date = DateTime.Now.AddDays(-5),
                Hours = 5,
                HourlyRate = 500,
                ActivityDescription = "Lecture Delivery"
            });

            Claims.TryAdd(claim.ClaimId, claim);
        }

        // GET: Claims
        // This is the action causing your 404. It MUST be public.
        public IActionResult Index()
        {
            // We pass the Lecturers dictionary so the View can look up names
            ViewBag.Lecturers = Lecturers;

            // Return the list of claims, sorted by date
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

            // Pass lecturer details for the header
            if (Lecturers.TryGetValue(claim.LecturerId, out var lecturer))
            {
                ViewBag.LecturerName = lecturer.FullName;
                ViewBag.HourlyRate = lecturer.HourlyRate;
            }
            else
            {
                ViewBag.LecturerName = "Unknown";
                ViewBag.HourlyRate = 0;
            }

            return View(claim);
        }

        // POST: Claims/AddItem
        [HttpPost]
        public IActionResult AddItem(Guid claimId, DateTime date, decimal hours, string activityDescription)
        {
            if (!Claims.TryGetValue(claimId, out var claim)) return NotFound();

            // Prevent editing if not in Draft
            if (claim.Status != ClaimStatus.Draft) return Unauthorized();

            // Find lecturer to get the rate
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

            return RedirectToAction("Edit", new { id = claimId });
        }

        // POST: Claims/Submit
        [HttpPost]
        public IActionResult Submit(Guid id)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();

            claim.Status = ClaimStatus.Submitted;
            claim.SubmittedAt = DateTime.UtcNow;

            return RedirectToAction("Index");
        }

        // POST: Claims/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(Guid id, IFormFile file)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();

            // Validation checks
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file.";
                return RedirectToAction("Edit", new { id });
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
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

            // Save file
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