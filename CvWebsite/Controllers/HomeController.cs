using CvWebsite.Context;
using CvWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CvWebsite.Controllers
{
    // The site's landing page. Renders the "About Me" section using the
    // first (and normally only) AboutMe record in the database, plus the
    // certificate cards shown further down the page.
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var aboutMe = await _context.AboutMe.AsNoTracking().FirstOrDefaultAsync();
            // Only the 4 most recent certificates are shown on the About Me
            // page so the section fits in one row without pushing the page
            // height up. The rest can still be seen from the admin list.
            ViewBag.Certificates = await _context.Certificate
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Take(4)
                .ToListAsync();

            return View(aboutMe);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
