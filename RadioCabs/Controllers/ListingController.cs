using Microsoft.AspNetCore.Mvc;
using RadioCabs.Helpers;
using RadioCabs.Models;

namespace RadioCabs.Controllers
{
    public class ListingController : Controller
    {
        private readonly ApplicationContext _context;

        public ListingController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string searchTerm = "")
        {
            var query = _context.Companies.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.CompanyName.Contains(searchTerm) || c.CitySafe().Contains(searchTerm));
            }

            var model = new ListingPageViewModel
            {
                SearchTerm = searchTerm,
                Results = query.OrderBy(c => c.CompanyName).ToList(),
                Registration = new Company()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(ListingPageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Results = _context.Companies.OrderBy(c => c.CompanyName).ToList();
                return View("Index", model);
            }

            if (_context.Companies.Any(c => c.CompanyUniqueId == model.Registration.CompanyUniqueId))
            {
                ModelState.AddModelError("Registration.CompanyUniqueId", "Company ID already exists.");
                model.Results = _context.Companies.OrderBy(c => c.CompanyName).ToList();
                return View("Index", model);
            }

            model.Registration.Password = PasswordHelper.HashPassword(model.Registration.Password);
            model.Registration.PaymentStatus = "Pending";
            model.Registration.PaymentAmount = PaymentCalculator.GetCompanyAmount(model.Registration.PaymentType ?? "Monthly");
            _context.Companies.Add(model.Registration);
            _context.SaveChanges();

            TempData["Success"] = "Company registration completed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }

    internal static class CompanyExtensions
    {
        public static string CitySafe(this Company company)
        {
            return company.Address ?? string.Empty;
        }
    }
}
