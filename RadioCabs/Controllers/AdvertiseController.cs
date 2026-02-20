using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RadioCabs.Helpers;
using RadioCabs.Models;

namespace RadioCabs.Controllers
{
    public class AdvertiseController : Controller
    {
        private readonly ApplicationContext _context;

        public AdvertiseController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int? companyId = HttpContext.Session.GetInt32("CompanyId");
            if (companyId == null)
            {
                TempData["Error"] = "Please login as a company before advertising.";
                return RedirectToAction("Login", "Company");
            }

            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == companyId.Value);
            if (company == null)
            {
                HttpContext.Session.Remove("CompanyId");
                HttpContext.Session.Remove("CompanyName");
                TempData["Error"] = "Please login as a valid company account before advertising.";
                return RedirectToAction("Login", "Company");
            }


            var model = new AdvertisePageViewModel
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName ?? string.Empty,
                CompanyUniqueId = company.CompanyUniqueId ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AdvertisePageViewModel model, string submitAction)
        {
            model.Advertisement ??= new Advertisement();
            int? loggedInCompanyId = HttpContext.Session.GetInt32("CompanyId");
            if (loggedInCompanyId == null)
            {
                TempData["Error"] = "Please login as a company before advertising.";
                return RedirectToAction("Login", "Company");
            }

            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == loggedInCompanyId.Value);
            if (company == null)
            {
                HttpContext.Session.Remove("CompanyId");
                HttpContext.Session.Remove("CompanyName");
                TempData["Error"] = "Please login as a valid company account before advertising.";
                return RedirectToAction("Login", "Company");
            }

            model.CompanyId = company.CompanyId;
            model.CompanyName = company.CompanyName ?? string.Empty;
            model.CompanyUniqueId = company.CompanyUniqueId ?? string.Empty;


            model.Advertisement.CompanyName = company.CompanyName;
            model.Advertisement.Address = company.Address;
            model.Advertisement.Mobile = company.Mobile;
            model.Advertisement.Telephone = company.Telephone;
            model.Advertisement.FaxNumber = company.FaxNumber;
            model.Advertisement.Email = company.Email;
            model.Advertisement.PaymentStatus = "Pending";
            model.Advertisement.PaymentAmount = PaymentCalculator.GetAdvertisementAmount(model.Advertisement.PaymentType ?? "Monthly");

            ModelState.Remove("Advertisement.CompanyName");
            ModelState.Remove("Advertisement.Address");
            ModelState.Remove("Advertisement.Mobile");
            ModelState.Remove("Advertisement.Telephone");
            ModelState.Remove("Advertisement.FaxNumber");
            ModelState.Remove("Advertisement.Email");
            ModelState.Remove("Advertisement.PaymentStatus");
            ModelState.Remove("Advertisement.PaymentAmount");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Advertisements.Add(model.Advertisement);
            _context.SaveChanges();

            if (string.Equals(submitAction, "PayNow", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Advertisement", "Payment", new { id = model.Advertisement.AdvertisementId });
            }


            TempData["Success"] = "Advertisement request submitted.";
            TempData["AdvertisementId"] = model.Advertisement.AdvertisementId;
            return RedirectToAction(nameof(Index));
        }
    }
}
