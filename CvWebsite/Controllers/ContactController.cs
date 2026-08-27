using CvWebsite.Models;
using CvWebsite.Context;
using Microsoft.AspNetCore.Mvc;

namespace CvWebsite.Controllers
{
    // Public "Contact" page — visitors fill this form, it gets saved to the
    // Contact table and shows up under Admin > Mesajlar.
    public class ContactController : BaseController
    {
        public ContactController(AppDbContext context) : base(context)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new Contact());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();

                ViewBag.Sent = true;
                return View(new Contact());
            }

            return View(contact);
        }
    }
}
