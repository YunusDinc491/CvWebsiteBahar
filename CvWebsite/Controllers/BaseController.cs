using CvWebsite.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CvWebsite.Controllers
{
    // Public-facing controllers inherit from this so every page has access
    // to the same "brand" info (name + title) for the navbar, without each
    // action having to fetch it itself.
    public abstract class BaseController : Controller
    {
        protected readonly AppDbContext _context;

        protected BaseController(AppDbContext context)
        {
            _context = context;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var aboutMe = await _context.AboutMe.AsNoTracking().FirstOrDefaultAsync();
            ViewData["BrandName"] = aboutMe?.NameSurname ?? "Add your name";
            ViewData["BrandTitle"] = aboutMe?.JobName ?? "Add your job title";

            await next();
        }
    }
}
