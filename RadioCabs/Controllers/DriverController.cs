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
        public IActionResult Index(string searchTerm = "", string city = "", int? minExperience = null, string paymentStatus = "")
        {
            var query = _context.Drivers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d =>
                    d.DriverName.Contains(searchTerm) ||
                    (d.City != null && d.City.Contains(searchTerm)) ||
                    (d.Email != null && d.Email.Contains(searchTerm)) ||
                    (d.Mobile != null && d.Mobile.Contains(searchTerm)));
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(d => d.City != null && d.City.Contains(city));
            }

            if (minExperience.HasValue && minExperience.Value > 0)
            {
                query = query.Where(d => d.Experience >= minExperience.Value);
            }

            if (!string.IsNullOrWhiteSpace(paymentStatus))
            {
                query = query.Where(d => d.PaymentStatus == paymentStatus);
            }
        

            var model = new DriverPageViewModel
            {
                SearchTerm = searchTerm,
                City = city,
                MinExperience = minExperience,
                PaymentStatus = paymentStatus,
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

            var driver = _context.Drivers.Find(driverId.Value);
            if (driver == null)
            {
                HttpContext.Session.Remove("DriverId");
                HttpContext.Session.Remove("DriverName");
                return RedirectToAction(nameof(Login));
            }

            return View(driver);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var driver = _context.Drivers.Find(driverId.Value);
            if (driver == null)
            {
                HttpContext.Session.Remove("DriverId");
                HttpContext.Session.Remove("DriverName");
                return RedirectToAction(nameof(Login));
            }

            var model = new DriverProfileEditViewModel
            {
                DriverId = driver.DriverId,
                DriverName = driver.DriverName,
                DriverUniqueId = driver.DriverUniqueId,
                ContactPerson = driver.ContactPerson,
                Address = driver.Address,
                City = driver.City,
                Mobile = driver.Mobile,
                Telephone = driver.Telephone,
                Email = driver.Email,
                Experience = driver.Experience,
                Description = driver.Description
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(DriverProfileEditViewModel model)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (driverId.Value != model.DriverId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingDriver = _context.Drivers.Find(driverId.Value);
            if (existingDriver == null)
            {
                HttpContext.Session.Remove("DriverId");
                HttpContext.Session.Remove("DriverName");
                return RedirectToAction(nameof(Login));
            }

            existingDriver.DriverName = model.DriverName;
            existingDriver.ContactPerson = model.ContactPerson;
            existingDriver.Address = model.Address;
            existingDriver.City = model.City;
            existingDriver.Mobile = model.Mobile;
            existingDriver.Telephone = model.Telephone;
            existingDriver.Email = model.Email;
            existingDriver.Experience = model.Experience;
            existingDriver.Description = model.Description;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                existingDriver.Password = PasswordHelper.HashPassword(model.NewPassword);
            }

            _context.SaveChanges();

            HttpContext.Session.SetString("DriverName", existingDriver.DriverName ?? string.Empty);
            TempData["Success"] = "Your profile has been updated successfully.";
            return RedirectToAction(nameof(Dashboard));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProfile()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var driver = _context.Drivers.Find(driverId.Value);
            if (driver == null)
            {
                HttpContext.Session.Remove("DriverId");
                HttpContext.Session.Remove("DriverName");
                return RedirectToAction(nameof(Login));
            }

            _context.Drivers.Remove(driver);
            _context.SaveChanges();

            HttpContext.Session.Remove("DriverId");
            HttpContext.Session.Remove("DriverName");
            TempData["Success"] = "Your driver profile has been deleted.";
            return RedirectToAction(nameof(Index));
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
