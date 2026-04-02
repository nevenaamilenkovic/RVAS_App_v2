using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RvasApp.Data;
using RvasApp.Konstante;
using RvasApp.Models;
using System.Security.Claims;

namespace RvasApp.Controllers
{
    [Authorize]
    public class LekcijaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;//interfejs koji nam ima WebRootPath i omogucava nam da dodjemo do wwwroot folderaa

        public LekcijaController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = nameof(Roles.Instruktor))]
        public async Task<IActionResult> Materijali(int id) //id->lekcijaId
        {
            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(instruktorId))
                return Forbid();

            var lekcija = await _context.Lekcije
                .Include(l => l.Kurs)
                .Include(l => l.Materijali)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lekcija == null)
                return NotFound();

            if (lekcija.Kurs?.InstruktorId != instruktorId)
                return Forbid();

            return View(lekcija);
        }

        [Authorize(Roles = nameof(Roles.Instruktor))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMaterijal(int id, IFormFile fajl, string? naziv) //id=lekcijaId
        {
            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(instruktorId))
                return Forbid();

            var lekcija = await _context.Lekcije
                .Include(l => l.Kurs)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lekcija == null)
                return NotFound();

            if (lekcija.Kurs?.InstruktorId != instruktorId)
                return Forbid();

            if (fajl == null || fajl.Length == 0)
            {
                ModelState.AddModelError("", "Moras izabrati fajl");
                return RedirectToAction(nameof(Materijali), new { id });
            }

            var ext = Path.GetExtension(fajl.FileName).ToLowerInvariant();
            var dozvoljene = new HashSet<string> { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".zip", ".rar", ".txt" };
            if (!dozvoljene.Contains(ext))
            {
                ModelState.AddModelError("", "Tip fajla nije dozvoljen.");
                return RedirectToAction(nameof(Materijali), new { id });
            }

            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "lekcije", id.ToString());
            Directory.CreateDirectory(uploadsRoot);

            var safeBaseName = Path.GetFileNameWithoutExtension(fajl.FileName);
            safeBaseName = string.Join("_", safeBaseName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim();
            if (string.IsNullOrWhiteSpace(safeBaseName))
                safeBaseName = "materijal";

            var storedFileName = $"{safeBaseName}_{Guid.NewGuid():N}{ext}";
            var physicalPath = Path.Combine(uploadsRoot, storedFileName);

            await using (var stream = System.IO.File.Create(physicalPath))
            {
                await fajl.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", "lekcije", id.ToString(), storedFileName).Replace("\\", "/");

            var materijal = new LekcijaMaterijali
            {
                LekcijaId = id,
                Naziv = string.IsNullOrWhiteSpace(naziv) ? Path.GetFileName(fajl.FileName) : naziv.Trim(),
                Putanja = relativePath,
                Tip = fajl.ContentType,
                Velicina = fajl.Length,
                InstruktorId = instruktorId
            };

            _context.Materijali.Add(materijal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Materijali), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> PreuzmiMaterijal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var materijal = await _context.Materijali
                .Include(m => m.Lekcija)
                    .ThenInclude(l => l!.Kurs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materijal == null)
                return NotFound();

            var kursId = materijal.Lekcija?.KursId;
            if (kursId == null)
                return NotFound();

            var isInstruktor = User.IsInRole(nameof(Roles.Instruktor));
            var isPolaznik = User.IsInRole(nameof(Roles.Polaznik));

            if (isInstruktor)
            {
                if (materijal.Lekcija?.Kurs?.InstruktorId != userId)
                    return Forbid();
            }
            else if (isPolaznik)
            {
                var upisan = await _context.Prijave.AnyAsync(p => p.KursId == kursId.Value && p.PolaznikId == userId);
                if (!upisan)
                    return Forbid();
            }
            else
            {
                return Forbid();
            }

            var relative = (materijal.Putanja ?? "").TrimStart('/');
            var physicalPath = Path.Combine(_env.WebRootPath, relative.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            var downloadName = materijal.Naziv;
            if (string.IsNullOrWhiteSpace(downloadName))
                downloadName = Path.GetFileName(physicalPath);

            if (!Path.HasExtension(downloadName))
                downloadName = $"{downloadName}{Path.GetExtension(physicalPath)}";

            var contentType = string.IsNullOrWhiteSpace(materijal.Tip) ? "application/octet-stream" : materijal.Tip;
            return PhysicalFile(physicalPath, contentType, fileDownloadName: downloadName, enableRangeProcessing: true);
        }
    }
}

