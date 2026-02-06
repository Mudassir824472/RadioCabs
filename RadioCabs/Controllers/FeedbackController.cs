using Microsoft.AspNetCore.Mvc;
using RadioCabs.Models;

namespace RadioCabs.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationContext _context;

        public FeedbackController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new Feedback());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Feedback model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Feedbacks.Add(model);
            _context.SaveChanges();
            TempData["Success"] = "Thanks! Your feedback has been recorded.";
            return RedirectToAction(nameof(Index));
        }
    }
}
