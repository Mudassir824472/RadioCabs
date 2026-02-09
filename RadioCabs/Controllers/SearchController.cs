using Microsoft.AspNetCore.Mvc;
using RadioCabs.Models;

namespace RadioCabs.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationContext _context;

        public SearchController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string searchTerm = "")
        {
            var model = new SearchResultsViewModel
            {
                SearchTerm = searchTerm
            };

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View(model);
            }

            model.Companies = _context.Companies
                .Where(c =>
                    c.CompanyName.Contains(searchTerm) ||
                    c.CompanyUniqueId.Contains(searchTerm) ||
                    c.Email.Contains(searchTerm) ||
                    c.Mobile.Contains(searchTerm))
                .OrderBy(c => c.CompanyName)
                .ToList();

            model.Drivers = _context.Drivers
                .Where(d =>
                    d.DriverName.Contains(searchTerm) ||
                    d.DriverUniqueId.Contains(searchTerm) ||
                    (d.City != null && d.City.Contains(searchTerm)) ||
                    (d.Email != null && d.Email.Contains(searchTerm)) ||
                    (d.Mobile != null && d.Mobile.Contains(searchTerm)))
                .OrderBy(d => d.DriverName)
                .ToList();

            model.Advertisements = _context.Advertisements
                .Where(a =>
                    a.CompanyName.Contains(searchTerm) ||
                    a.Email.Contains(searchTerm) ||
                    a.Mobile.Contains(searchTerm) ||
                    a.Description.Contains(searchTerm))
                .OrderByDescending(a => a.AdvertisementId)
                .ToList();

            model.Feedbacks = _context.Feedbacks
                .Where(f =>
                    f.Name.Contains(searchTerm) ||
                    f.Email.Contains(searchTerm) ||
                    f.Mobile.Contains(searchTerm) ||
                    f.Description.Contains(searchTerm))
                .OrderByDescending(f => f.FeedbackId)
                .ToList();

            return View(model);
        }
    }
}
