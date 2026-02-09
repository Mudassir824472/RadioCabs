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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(DriverLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var passwordHash = PasswordHelper.HashPassword(model.Password);
            var driver = _context.Drivers.FirstOrDefault(d =>
                d.DriverUniqueId == model.DriverUniqueId &&
                d.Password == passwordHash);

            if (driver == null)
            {
                ModelState.AddModelError("", "Invalid Driver ID or Password.");
                return View(model);
            }

            HttpContext.Session.SetInt32("DriverId", driver.DriverId);
            HttpContext.Session.SetString("DriverName", driver.DriverName ?? "");

            return RedirectToAction(nameof(Dashboard));
        }

        public IActionResult Dashboard()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            ViewBag.DriverName = HttpContext.Session.GetString("DriverName");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("DriverId");
            HttpContext.Session.Remove("DriverName");
            return RedirectToAction(nameof(Login));
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
            model.PaymentAmount = PaymentCalculator.GetDriverAmount(model.PaymentType ?? "Monthly");
            _context.Drivers.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Driver registration completed successfully.";
            TempData["DriverId"] = model.DriverId;
            return RedirectToAction(nameof(Index));
        }
    }
}
