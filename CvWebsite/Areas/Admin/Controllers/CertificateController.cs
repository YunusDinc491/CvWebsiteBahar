using CvWebsite.Context;
using CvWebsite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CertificateController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg" };
        private const long MaxPhotoBytes = 5 * 1024 * 1024; // 5 MB

        public CertificateController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Certificate
        public async Task<IActionResult> Index()
        {
            return View(await _context.Certificate.OrderBy(c => c.Id).ToListAsync());
        }

        // GET: Admin/Certificate/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var certificate = await _context.Certificate.FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null) return NotFound();

            return View(certificate);
        }

        // GET: Admin/Certificate/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Certificate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Certificate certificate, IFormFile? photoFile)
        {
            if (ModelState.IsValid)
            {
                if (photoFile != null && photoFile.Length > 0)
                {
                    var savedPath = await SavePhotoAsync(photoFile);
                    if (savedPath == null)
                    {
                        ModelState.AddModelError(string.Empty, "Sadece PNG veya JPG formatında, 5 MB'tan küçük bir fotoğraf yükleyebilirsin.");
                        return View(certificate);
                    }
                    certificate.PhotoUrl = savedPath;
                }

                _context.Add(certificate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(certificate);
        }

        // GET: Admin/Certificate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var certificate = await _context.Certificate.FindAsync(id);
            if (certificate == null) return NotFound();

            return View(certificate);
        }

        // POST: Admin/Certificate/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Certificate certificate, IFormFile? photoFile)
        {
            if (id != certificate.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        var savedPath = await SavePhotoAsync(photoFile);
                        if (savedPath == null)
                        {
                            ModelState.AddModelError(string.Empty, "Sadece PNG veya JPG formatında, 5 MB'tan küçük bir fotoğraf yükleyebilirsin.");
                            return View(certificate);
                        }

                        DeletePhotoIfExists(certificate.PhotoUrl);
                        certificate.PhotoUrl = savedPath;
                    }
                    else
                    {
                        var existing = await _context.Certificate.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                        certificate.PhotoUrl = existing?.PhotoUrl;
                    }

                    _context.Update(certificate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificateExists(certificate.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(certificate);
        }

        // GET: Admin/Certificate/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var certificate = await _context.Certificate.FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null) return NotFound();

            return View(certificate);
        }

        // POST: Admin/Certificate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificate = await _context.Certificate.FindAsync(id);
            if (certificate != null)
            {
                DeletePhotoIfExists(certificate.PhotoUrl);
                _context.Certificate.Remove(certificate);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificate.Any(e => e.Id == id);
        }

        private async Task<string?> SavePhotoAsync(IFormFile photoFile)
        {
            var extension = Path.GetExtension(photoFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension) || photoFile.Length > MaxPhotoBytes)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }

        private void DeletePhotoIfExists(string? photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl) || !photoUrl.StartsWith("/uploads/"))
            {
                return;
            }

            var physicalPath = Path.Combine(_environment.WebRootPath, photoUrl.TrimStart('/'));
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }
    }
}
