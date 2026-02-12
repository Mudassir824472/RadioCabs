using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RadioCabs.Helpers;
using RadioCabs.Models;
using RadioCabs.Services;

namespace RadioCabs.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IPaymentGateway _paymentGateway;

        public PaymentController(ApplicationContext context, IPaymentGateway paymentGateway)
        {
            _context = context;
            _paymentGateway = paymentGateway;
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
        public async Task<IActionResult> Company(PaymentPageViewModel model)
        {
            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == model.EntityId);
            if (company == null)
            {
                return NotFound();
            }

            company.PaymentType = model.PaymentType;
            company.PaymentAmount = PaymentCalculator.GetCompanyAmount(model.PaymentType);

            if (!_paymentGateway.IsConfigured)
            {
                company.PaymentStatus = "Paid";
                _context.SaveChanges();
                TempData["Success"] = "Payment gateway is not configured. Company payment was marked as paid for offline processing.";
                return RedirectToAction("Profile", "Company");
            }

            _context.SaveChanges();


            var checkoutRequest = new PaymentCheckoutRequest
            {
                Section = "Company",
                EntityId = company.CompanyId,
                Name = company.CompanyName,
                PaymentType = company.PaymentType ?? "Monthly",
                Amount = company.PaymentAmount,
                SuccessUrl = BuildSuccessUrl(),
                CancelUrl = BuildCancelUrl("Company", company.CompanyId)
            };

            var session = await _paymentGateway.CreateCheckoutSessionAsync(checkoutRequest, HttpContext.RequestAborted);
            return Redirect(session.CheckoutUrl);

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
        public async Task<IActionResult> Driver(PaymentPageViewModel model)
        {
            var driver = _context.Drivers.FirstOrDefault(d => d.DriverId == model.EntityId);
            if (driver == null)
            {
                return NotFound();
            }

            driver.PaymentType = model.PaymentType;
            driver.PaymentAmount = PaymentCalculator.GetDriverAmount(model.PaymentType);

            if (!_paymentGateway.IsConfigured)
            {
                driver.PaymentStatus = "Paid";
                _context.SaveChanges();
                TempData["Success"] = "Payment gateway is not configured. Driver payment was marked as paid for offline processing.";
                return RedirectToAction("Index", "Driver");
            }   

            
            _context.SaveChanges();

            var checkoutRequest = new PaymentCheckoutRequest
            {
                Section = "Driver",
                EntityId = driver.DriverId,
                Name = driver.DriverName,
                PaymentType = driver.PaymentType ?? "Monthly",
                Amount = driver.PaymentAmount,
                SuccessUrl = BuildSuccessUrl(),
                CancelUrl = BuildCancelUrl("Driver", driver.DriverId)
            };

            var session = await _paymentGateway.CreateCheckoutSessionAsync(checkoutRequest, HttpContext.RequestAborted);
            return Redirect(session.CheckoutUrl);
        
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
        public async Task<IActionResult> Advertisement(PaymentPageViewModel model)
        {
            var advertisement = _context.Advertisements.FirstOrDefault(a => a.AdvertisementId == model.EntityId);
            if (advertisement == null)
            {
                return NotFound();
            }

            advertisement.PaymentType = model.PaymentType;
            advertisement.PaymentAmount = PaymentCalculator.GetAdvertisementAmount(model.PaymentType);

            if (!_paymentGateway.IsConfigured)
            {
                advertisement.PaymentStatus = "Paid";
                _context.SaveChanges();
                TempData["Success"] = "Payment gateway is not configured. Advertisement payment was marked as paid for offline processing.";
                return RedirectToAction("Index", "Advertise");
            }

            
            _context.SaveChanges();

            var checkoutRequest = new PaymentCheckoutRequest
            {
                Section = "Advertisement",
                EntityId = advertisement.AdvertisementId,
                Name = advertisement.CompanyName,
                PaymentType = advertisement.PaymentType ?? "Monthly",
                Amount = advertisement.PaymentAmount,
                SuccessUrl = BuildSuccessUrl(),
                CancelUrl = BuildCancelUrl("Advertisement", advertisement.AdvertisementId)
            };

            var session = await _paymentGateway.CreateCheckoutSessionAsync(checkoutRequest, HttpContext.RequestAborted);
            return Redirect(session.CheckoutUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Complete(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return BadRequest();
            }

            var session = await _paymentGateway.GetCheckoutSessionAsync(sessionId, HttpContext.RequestAborted);
            if (!string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Payment was not completed.";
                return RedirectToAction("Index", "Home");
            }

            if (!session.Metadata.TryGetValue("section", out var section) ||
                !session.Metadata.TryGetValue("entityId", out var entityIdValue) ||
                !int.TryParse(entityIdValue, out var entityId))
            {
                TempData["Error"] = "Payment metadata was incomplete.";
                return RedirectToAction("Index", "Home");
            }

            var paymentType = session.Metadata.TryGetValue("paymentType", out var storedPaymentType)
                ? storedPaymentType
                : "Monthly";

            switch (section)
            {
                case "Company":
                    var company = _context.Companies.FirstOrDefault(c => c.CompanyId == entityId);
                    if (company == null)
                    {
                        return NotFound();
                    }

                    company.PaymentType = paymentType;
                    company.PaymentAmount = PaymentCalculator.GetCompanyAmount(paymentType);
                    company.PaymentStatus = "Paid";
                    _context.SaveChanges();
                    TempData["Success"] = "Company payment completed successfully.";
                    return RedirectToAction("Profile", "Company");
                case "Driver":
                    var driver = _context.Drivers.FirstOrDefault(d => d.DriverId == entityId);
                    if (driver == null)
                    {
                        return NotFound();
                    }

                    driver.PaymentType = paymentType;
                    driver.PaymentAmount = PaymentCalculator.GetDriverAmount(paymentType);
                    driver.PaymentStatus = "Paid";
                    _context.SaveChanges();
                    TempData["Success"] = "Driver payment completed successfully.";
                    return RedirectToAction("Index", "Driver");
                case "Advertisement":
                    var advertisement = _context.Advertisements.FirstOrDefault(a => a.AdvertisementId == entityId);
                    if (advertisement == null)
                    {
                        return NotFound();
                    }

                    advertisement.PaymentType = paymentType;
                    advertisement.PaymentAmount = PaymentCalculator.GetAdvertisementAmount(paymentType);
                    advertisement.PaymentStatus = "Paid";
                    _context.SaveChanges();
                    TempData["Success"] = "Advertisement payment completed successfully.";
                    return RedirectToAction("Index", "Advertise");
                default:
                    TempData["Error"] = "Payment section is invalid.";
                    return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Cancel(string section, int entityId)
        {
            TempData["Error"] = "Payment was cancelled.";
            return section switch
            {
                "Company" => RedirectToAction(nameof(Company), new { id = entityId }),
                "Driver" => RedirectToAction(nameof(Driver), new { id = entityId }),
                "Advertisement" => RedirectToAction(nameof(Advertisement), new { id = entityId }),
                _ => RedirectToAction("Index", "Home")
            };
        }

        private string BuildSuccessUrl()
        {
            return Url.Action(nameof(Complete), "Payment", new { sessionId = "{CHECKOUT_SESSION_ID}" }, Request.Scheme)
                   ?? string.Empty;
        }

        private string BuildCancelUrl(string section, int entityId)
        {
            return Url.Action(nameof(Cancel), "Payment", new { section, entityId }, Request.Scheme)
                   ?? string.Empty;
        }
    }
}
        
