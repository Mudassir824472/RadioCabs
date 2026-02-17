using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RadioCabs.Models;

namespace RadioCabs.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationContext _context;

        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                PaidAdvertisements = _context.Advertisements
                                .Where(a => a.PaymentStatus == "Paid")
                                .OrderByDescending(a => a.AdvertisementId)
                                .Take(6)
                                .ToList()
            };

            return View(model);
        }

       

        public IActionResult LoginOptions()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterOptions()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
