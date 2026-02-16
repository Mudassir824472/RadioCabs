using System;
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
                PaymentStatus = company.PaymentStatus ?? "Pending",
                ExpiryYear = DateTime.UtcNow.Year
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

      
            if (!ModelState.IsValid)
            {
                model.Section = "Company";
                model.Name = company.CompanyName;
                model.PaymentStatus = company.PaymentStatus ?? "Pending";
                model.PaymentAmount = PaymentCalculator.GetCompanyAmount(model.PaymentType ?? company.PaymentType ?? "Monthly");
                return View("Pay", model);
            }

            company.PaymentType = model.PaymentType;
            company.PaymentAmount = PaymentCalculator.GetCompanyAmount(model.PaymentType);
            company.PaymentStatus = "Paid";

            _context.SaveChanges();

            TempData["Success"] = "Company payment completed successfully.";
            return RedirectToAction(nameof(Receipt), new { section = "Company", id = company.CompanyId });
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
                PaymentStatus = driver.PaymentStatus ?? "Pending",
                ExpiryYear = DateTime.UtcNow.Year
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

            if (!ModelState.IsValid)
            {
                model.Section = "Driver";
                model.Name = driver.DriverName;
                model.PaymentStatus = driver.PaymentStatus ?? "Pending";
                model.PaymentAmount = PaymentCalculator.GetDriverAmount(model.PaymentType ?? driver.PaymentType ?? "Monthly");
                return View("Pay", model);
            }

            driver.PaymentType = model.PaymentType;
            driver.PaymentAmount = PaymentCalculator.GetDriverAmount(model.PaymentType);
            driver.PaymentStatus = "Paid";

            _context.SaveChanges();


            TempData["Success"] = "Driver payment completed successfully.";
            return RedirectToAction(nameof(Receipt), new { section = "Driver", id = driver.DriverId });


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
                PaymentStatus = advertisement.PaymentStatus ?? "Pending",
                ExpiryYear = DateTime.UtcNow.Year
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


            if (!ModelState.IsValid)
            {
                model.Section = "Advertisement";
                model.Name = advertisement.CompanyName;
                model.PaymentStatus = advertisement.PaymentStatus ?? "Pending";
                model.PaymentAmount = PaymentCalculator.GetAdvertisementAmount(model.PaymentType ?? advertisement.PaymentType ?? "Monthly");
                return View("Pay", model);
            }

            advertisement.PaymentType = model.PaymentType;
            advertisement.PaymentAmount = PaymentCalculator.GetAdvertisementAmount(model.PaymentType);
            advertisement.PaymentStatus = "Paid";

            _context.SaveChanges();


            TempData["Success"] = "Advertisement payment completed successfully.";
            return RedirectToAction(nameof(Receipt), new { section = "Advertisement", id = advertisement.AdvertisementId });

        }
        public IActionResult Receipt(string section, int id)
        {
            if (string.IsNullOrWhiteSpace(section) || id <= 0)
            {
                return BadRequest();
            }

            PaymentPageViewModel? model = null;
            var normalizedSection = section.Trim();

            if (normalizedSection.Equals("Company", StringComparison.OrdinalIgnoreCase))
            {
                var company = _context.Companies.FirstOrDefault(c => c.CompanyId == id);
                if (company == null)
                {
                    return NotFound();
                }

                model = new PaymentPageViewModel
                {
                    Section = "Company",
                    EntityId = company.CompanyId,
                    Name = company.CompanyName,
                    PaymentType = company.PaymentType ?? "Monthly",
                    PaymentAmount = company.PaymentAmount,
                    PaymentStatus = company.PaymentStatus ?? "Pending"
                };
            }
            else if (normalizedSection.Equals("Driver", StringComparison.OrdinalIgnoreCase))
            {
                var driver = _context.Drivers.FirstOrDefault(d => d.DriverId == id);
                if (driver == null)
                {
                    return NotFound();
                }

                model = new PaymentPageViewModel
                {
                    Section = "Driver",
                    EntityId = driver.DriverId,
                    Name = driver.DriverName,
                    PaymentType = driver.PaymentType ?? "Monthly",
                    PaymentAmount = driver.PaymentAmount,
                    PaymentStatus = driver.PaymentStatus ?? "Pending"
                };
            }
            else if (normalizedSection.Equals("Advertisement", StringComparison.OrdinalIgnoreCase))
            {
                var advertisement = _context.Advertisements.FirstOrDefault(a => a.AdvertisementId == id);
                if (advertisement == null)
                {
                    return NotFound();
                }

                model = new PaymentPageViewModel
                {
                    Section = "Advertisement",
                    EntityId = advertisement.AdvertisementId,
                    Name = advertisement.CompanyName,
                    PaymentType = advertisement.PaymentType ?? "Monthly",
                    PaymentAmount = advertisement.PaymentAmount,
                    PaymentStatus = advertisement.PaymentStatus ?? "Pending"
                };
            }
            else
            {
                return BadRequest();
            }

            return View(model);
        }

    }
}
        
