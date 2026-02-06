using Microsoft.AspNetCore.Mvc;
using RadioCabs.Models;

namespace RadioCabs.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationContext _context;

        public AdminController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin123")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction(nameof(Dashboard));
            }

            ViewData["Error"] = "Invalid admin credentials.";
            return View();
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction(nameof(Login));
            }

            var model = new AdminDashboardViewModel
            {
                Companies = _context.Companies.OrderByDescending(c => c.CompanyId).ToList(),
                Drivers = _context.Drivers.OrderByDescending(d => d.DriverId).ToList(),
                Advertisements = _context.Advertisements.OrderByDescending(a => a.AdvertisementId).ToList(),
                Feedbacks = _context.Feedbacks.OrderByDescending(f => f.FeedbackId).ToList()
            };

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction(nameof(Login));
        }
    }
}
