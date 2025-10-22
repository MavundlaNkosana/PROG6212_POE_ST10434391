using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Contract_Monthly_Claim_System.Models;


namespace Contract_Monthly_Claim_System.Controllers
{
    public class ClaimsController : Controller
    {
        // In-memory stores for prototype
        private static ConcurrentDictionary<Guid, Claim> Claims = new();
        private static ConcurrentDictionary<Guid, Lecturer> Lecturers = new();

        // Seed a lecturer for demo
        static ClaimsController()
        {
            var l = new Lecturer { StaffNumber = "L001", FullName = "Dr. Thabo", Email = "thabo@uni.edu", HourlyRate = 500.00m };
            Lecturers.TryAdd(l.LecturerId, l);
        }

        public IActionResult Index()
        {
            return View(Claims.Values.OrderByDescending(c => c.SubmittedAt));
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
            ViewBag.Lecturers = Lecturers.Values;
            return View(claim);
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
        public IActionResult ClockIn(Guid id, DateTime start, DateTime end)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();
            var lecturer = Lecturers[claim.LecturerId];
            var item = new ClaimItem
            {
                ClaimId = claim.ClaimId,
                Date = start.Date,
                Hours = (decimal)(end - start).TotalHours,
                HourlyRate = lecturer.HourlyRate,
                ActivityDescription = "Clocked session"
            };
            claim.Items.Add(item);
            return Json(new { success = true, hours = item.Hours, amount = item.Amount });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Guid id, IFormFile file)
        {
            if (!Claims.TryGetValue(id, out var claim)) return NotFound();
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploads);
                var filename = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filepath = Path.Combine(uploads, filename);
                using (var fs = System.IO.File.Create(filepath))
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
            }
            return RedirectToAction("Edit", new { id });
        }
    }

}
