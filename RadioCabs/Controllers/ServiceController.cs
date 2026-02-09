using Microsoft.AspNetCore.Mvc;

namespace RadioCabs.Controllers;

public class ServicesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
