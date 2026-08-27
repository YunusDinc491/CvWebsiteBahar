using CvWebsite.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvWebsite.Controllers
{
    // Public "Resume" page — lists work experience entered in the admin panel.
    public class ResumeController : BaseController
    {
        public ResumeController(AppDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var experience = await _context.Resume
                .AsNoTracking()
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            return View(experience);
        }
    }
}
