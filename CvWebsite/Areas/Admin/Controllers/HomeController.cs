using Microsoft.AspNetCore.Mvc;

namespace CvWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public IActionResult Index()
        {
            return View();
        }
    }
}
