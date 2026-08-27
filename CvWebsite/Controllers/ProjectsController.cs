using CvWebsite.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvWebsite.Controllers
{
    // Public "Projects" page — lists the projects entered in the admin panel.
    // Named "Projects" (plural) so the route is /Projects, matching the nav link.
    public class ProjectsController : BaseController
    {
        public ProjectsController(AppDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Project
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(projects);
        }
    }
}
