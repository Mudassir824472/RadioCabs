using Microsoft.AspNetCore.Mvc;
using RadioCabs.Helpers;
using RadioCabs.Models;

namespace RadioCabs.Controllers
{
    public class DriverController : Controller
    {
        private readonly ApplicationContext _context;

        public DriverController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string searchTerm = "")
        {
            var query = _context.Drivers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.DriverName.Contains(searchTerm) || d.City.Contains(searchTerm));
            }

            var model = new DriverPageViewModel
            {
                SearchTerm = searchTerm,
                Results = query.OrderBy(d => d.DriverName).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new Driver());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Driver model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.Drivers.Any(d => d.DriverUniqueId == model.DriverUniqueId))
            {
                ModelState.AddModelError("DriverUniqueId", "Driver ID already exists.");
                return View(model);
            }

            model.Password = PasswordHelper.HashPassword(model.Password);
            model.PaymentStatus = "Pending";
            _context.Drivers.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Driver registration completed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
