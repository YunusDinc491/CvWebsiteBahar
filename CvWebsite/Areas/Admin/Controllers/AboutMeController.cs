using CvWebsite.Context;
using CvWebsite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AboutMeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg" };
        private const long MaxPhotoBytes = 5 * 1024 * 1024; // 5 MB

        public AboutMeController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/AboutMe
        public async Task<IActionResult> Index()
        {
            return View(await _context.AboutMe.ToListAsync());
        }

        // GET: Admin/AboutMe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var aboutMe = await _context.AboutMe.FirstOrDefaultAsync(m => m.Id == id);
            if (aboutMe == null) return NotFound();

            return View(aboutMe);
        }

        // GET: Admin/AboutMe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AboutMe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutMe aboutMe, IFormFile? photoFile)
        {
            if (ModelState.IsValid)
            {
                if (photoFile != null && photoFile.Length > 0)
                {
                    var savedPath = await SavePhotoAsync(photoFile);
                    if (savedPath == null)
                    {
                        ModelState.AddModelError(string.Empty, "Sadece PNG veya JPG formatında, 5 MB'tan küçük bir fotoğraf yükleyebilirsin.");
                        return View(aboutMe);
                    }
                    aboutMe.PhotoUrl = savedPath;
                }

                _context.Add(aboutMe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aboutMe);
        }

        // GET: Admin/AboutMe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var aboutMe = await _context.AboutMe.FindAsync(id);
            if (aboutMe == null) return NotFound();

            return View(aboutMe);
        }

        // POST: Admin/AboutMe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AboutMe aboutMe, IFormFile? photoFile)
        {
            if (id != aboutMe.Id) return NotFound();

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
                            return View(aboutMe);
                        }

                        // Eski fotoğrafı diskten sil (kendi yüklediğimiz dosyaysa)
                        DeletePhotoIfExists(aboutMe.PhotoUrl);
                        aboutMe.PhotoUrl = savedPath;
                    }
                    else
                    {
                        // Yeni dosya seçilmediyse mevcut fotoğrafı koru
                        var existing = await _context.AboutMe.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                        aboutMe.PhotoUrl = existing?.PhotoUrl;
                    }

                    _context.Update(aboutMe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutMeExists(aboutMe.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aboutMe);
        }

        // GET: Admin/AboutMe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var aboutMe = await _context.AboutMe.FirstOrDefaultAsync(m => m.Id == id);
            if (aboutMe == null) return NotFound();

            return View(aboutMe);
        }

        // POST: Admin/AboutMe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aboutMe = await _context.AboutMe.FindAsync(id);
            if (aboutMe != null)
            {
                DeletePhotoIfExists(aboutMe.PhotoUrl);
                _context.AboutMe.Remove(aboutMe);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AboutMeExists(int id)
        {
            return _context.AboutMe.Any(e => e.Id == id);
        }

        // Saves the uploaded file under wwwroot/uploads with a unique name
        // and returns the relative URL to store in the database
        // (e.g. "/uploads/xxxxx.png"), or null if the file is invalid.
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

        // Deletes a previously uploaded photo from wwwroot/uploads, if the
        // given url points to one (leaves external URLs / placeholders alone).
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
