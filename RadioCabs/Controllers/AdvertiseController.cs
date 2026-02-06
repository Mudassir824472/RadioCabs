using Microsoft.AspNetCore.Mvc;
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
            var model = new AdvertisePageViewModel
            {
                Companies = _context.Companies.OrderBy(c => c.CompanyName).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AdvertisePageViewModel model)
        {
            model.Companies = _context.Companies.OrderBy(c => c.CompanyName).ToList();

            if (model.CompanyId == null)
            {
                ModelState.AddModelError("CompanyId", "Please select a registered company.");
                return View(model);
            }

            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == model.CompanyId);
            if (company == null)
            {
                ModelState.AddModelError("CompanyId", "Selected company was not found.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Advertisement.CompanyName = company.CompanyName;
            model.Advertisement.Address = company.Address;
            model.Advertisement.Mobile = company.Mobile;
            model.Advertisement.Telephone = company.Telephone;
            model.Advertisement.FaxNumber = company.FaxNumber;
            model.Advertisement.Email = company.Email;
            model.Advertisement.PaymentStatus = "Pending";

            _context.Advertisements.Add(model.Advertisement);
            _context.SaveChanges();
            TempData["Success"] = "Advertisement request submitted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
