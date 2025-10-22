using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;

namespace Contract_Monthly_Claim_System.Controllers
{
    public class ClaimsController : Controller
    {
        // In-memory stores for prototype
        public static ConcurrentDictionary<Guid, Claim> Claims = new();
        public static ConcurrentDictionary<Guid, Lecturer> Lecturers = new();

        // Seeding remains the same...
        static ClaimsController()
        {
            var lecturer = new Lecturer { StaffNumber = "L001", FullName = "Dr. Thabo", Email = "thabo@uni.edu", HourlyRate = 500.00m };
            Lecturers.TryAdd(lecturer.LecturerId, lecturer);

            var draftClaim = new Claim { LecturerId = lecturer.LecturerId, Month = 10, Year = 2025, Status = ClaimStatus.Draft };
            draftClaim.Items.Add(new ClaimItem { Date = new DateTime(2025, 10, 5), Hours = 3, HourlyRate = 500, ActivityDescription = "Grading assignments" });
            Claims.TryAdd(draftClaim.ClaimId, draftClaim);
        }

        public IActionResult Index() => View(Claims.Values.OrderByDescending(c => c.Year).ThenByDescending(c => c.Month));

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
            ViewBag.HourlyRate = lecturer.HourlyRate;

            // Pass any error/success messages to the view
            ViewBag.UploadMessage = TempData["UploadMessage"];
            ViewBag.UploadError = TempData["UploadError"];

            return View(claim);
        }

        [HttpPost]
        public IActionResult AddItem(Guid claimId, DateTime date, decimal hours, string activityDescription)
        {
            if (!Claims.TryGetValue(claimId, out var claim)) return NotFound();
            if (claim.Status != ClaimStatus.Draft) return Unauthorized();

            var lecturer = Lecturers[claim.LecturerId];
            claim.Items.Add(new ClaimItem
            {
                ClaimId = claimId,
                Date = date,
                Hours = hours,
                HourlyRate = lecturer.HourlyRate,
                ActivityDescription = activityDescription
            });
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

        // UPDATED: Secure Upload Action
        [HttpPost]
        public async Task<IActionResult> Upload(Guid id, IFormFile file)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();
            if (claim.Status != ClaimStatus.Draft)
            {
                TempData["UploadError"] = "Cannot upload documents to a claim that has already been submitted.";
                return RedirectToAction("Edit", new { id });
            }

            // 1. File validation
            if (file == null || file.Length == 0)
            {
                TempData["UploadError"] = "Please select a file to upload.";
                return RedirectToAction("Edit", new { id });
            }

            // 2. File size limit (e.g., 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                TempData["UploadError"] = "File size cannot exceed 5MB.";
                return RedirectToAction("Edit", new { id });
            }

            // 3. File type validation
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            {
                TempData["UploadError"] = "Invalid file type. Only PDF, DOCX, and XLSX files are allowed.";
                return RedirectToAction("Edit", new { id });
            }

            // 4. Secure storage
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolderPath); // Ensure the directory exists

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Link to claim
            claim.Documents.Add(new SupportingDocument
            {
                ClaimId = id,
                FileName = file.FileName,
                FileUrl = $"/uploads/{uniqueFileName}",
                ContentType = file.ContentType,
                FileSize = file.Length
            });

            TempData["UploadMessage"] = $"File '{file.FileName}' uploaded successfully.";
            return RedirectToAction("Edit", new { id });
        }
    }
}

