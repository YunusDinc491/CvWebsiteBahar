using CvWebsite.Context;
using CvWebsite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg" };
        private const long MaxPhotoBytes = 5 * 1024 * 1024; // 5 MB

        public ProjectController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Project
        public async Task<IActionResult> Index()
        {
            return View(await _context.Project.ToListAsync());
        }

        // GET: Admin/Project/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Project.FirstOrDefaultAsync(m => m.Id == id);
            if (project == null) return NotFound();

            return View(project);
        }

        // GET: Admin/Project/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project, IFormFile? photoFile)
        {
            if (ModelState.IsValid)
            {
                if (photoFile != null && photoFile.Length > 0)
                {
                    var savedPath = await SavePhotoAsync(photoFile);
                    if (savedPath == null)
                    {
                        ModelState.AddModelError(string.Empty, "Sadece PNG veya JPG formatında, 5 MB'tan küçük bir fotoğraf yükleyebilirsin.");
                        return View(project);
                    }
                    project.PhotoUrl = savedPath;
                }

                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Admin/Project/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Project.FindAsync(id);
            if (project == null) return NotFound();

            return View(project);
        }

        // POST: Admin/Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project, IFormFile? photoFile)
        {
            if (id != project.Id) return NotFound();

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
                            return View(project);
                        }

                        DeletePhotoIfExists(project.PhotoUrl);
                        project.PhotoUrl = savedPath;
                    }
                    else
                    {
                        var existing = await _context.Project.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                        project.PhotoUrl = existing?.PhotoUrl;
                    }

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Admin/Project/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Project.FirstOrDefaultAsync(m => m.Id == id);
            if (project == null) return NotFound();

            return View(project);
        }

        // POST: Admin/Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project != null)
            {
                DeletePhotoIfExists(project.PhotoUrl);
                _context.Project.Remove(project);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
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
