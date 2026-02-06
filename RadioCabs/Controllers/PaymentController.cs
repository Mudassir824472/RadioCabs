using Microsoft.AspNetCore.Mvc;
using RadioCabs.Helpers;
using RadioCabs.Models;

namespace RadioCabs.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationContext _context;

        public PaymentController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Company(int id)
        {
            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            var model = new PaymentPageViewModel
            {
                Section = "Company",
                EntityId = company.CompanyId,
                Name = company.CompanyName,
                PaymentType = company.PaymentType ?? "Monthly",
                PaymentAmount = PaymentCalculator.GetCompanyAmount(company.PaymentType ?? "Monthly"),
                PaymentStatus = company.PaymentStatus ?? "Pending"
            };

            return View("Pay", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Company(PaymentPageViewModel model)
        {
            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == model.EntityId);
            if (company == null)
            {
                return NotFound();
            }

            company.PaymentType = model.PaymentType;
            company.PaymentAmount = PaymentCalculator.GetCompanyAmount(model.PaymentType);
            company.PaymentStatus = "Paid";
            _context.SaveChanges();

            TempData["Success"] = "Company payment completed successfully.";
            return RedirectToAction("Profile", "Company");
        }

        [HttpGet]
        public IActionResult Driver(int id)
        {
            var driver = _context.Drivers.FirstOrDefault(d => d.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }

            var model = new PaymentPageViewModel
            {
                Section = "Driver",
                EntityId = driver.DriverId,
                Name = driver.DriverName,
                PaymentType = driver.PaymentType ?? "Monthly",
                PaymentAmount = PaymentCalculator.GetDriverAmount(driver.PaymentType ?? "Monthly"),
                PaymentStatus = driver.PaymentStatus ?? "Pending"
            };

            return View("Pay", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Driver(PaymentPageViewModel model)
        {
            var driver = _context.Drivers.FirstOrDefault(d => d.DriverId == model.EntityId);
            if (driver == null)
            {
                return NotFound();
            }

            driver.PaymentType = model.PaymentType;
            driver.PaymentAmount = PaymentCalculator.GetDriverAmount(model.PaymentType);
            driver.PaymentStatus = "Paid";
            _context.SaveChanges();

            TempData["Success"] = "Driver payment completed successfully.";
            return RedirectToAction("Index", "Driver");
        }

        [HttpGet]
        public IActionResult Advertisement(int id)
        {
            var advertisement = _context.Advertisements.FirstOrDefault(a => a.AdvertisementId == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            var model = new PaymentPageViewModel
            {
                Section = "Advertisement",
                EntityId = advertisement.AdvertisementId,
                Name = advertisement.CompanyName,
                PaymentType = advertisement.PaymentType ?? "Monthly",
                PaymentAmount = PaymentCalculator.GetAdvertisementAmount(advertisement.PaymentType ?? "Monthly"),
                PaymentStatus = advertisement.PaymentStatus ?? "Pending"
            };

            return View("Pay", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Advertisement(PaymentPageViewModel model)
        {
            var advertisement = _context.Advertisements.FirstOrDefault(a => a.AdvertisementId == model.EntityId);
            if (advertisement == null)
            {
                return NotFound();
            }

            advertisement.PaymentType = model.PaymentType;
            advertisement.PaymentAmount = PaymentCalculator.GetAdvertisementAmount(model.PaymentType);
            advertisement.PaymentStatus = "Paid";
            _context.SaveChanges();

            TempData["Success"] = "Advertisement payment completed successfully.";
            return RedirectToAction("Index", "Advertise");
        }
    }
}
