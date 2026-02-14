using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioCabs.Helpers;
using RadioCabs.Models;


namespace RadioCabs.Controllers
{

    public class CompanyController : Controller
    {
        private readonly ApplicationContext _context;

        public CompanyController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: /Company/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Company/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Company company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }

            // ✅ Check for duplicate CompanyUniqueId
            bool isDuplicate = _context.Companies
                .Any(c => c.CompanyUniqueId == company.CompanyUniqueId);

            if (isDuplicate)
            {
                ModelState.AddModelError("CompanyUniqueId", "This Company ID is already taken.");
                return View(company);
            }

            // HASH PASSWORD
            company.Password = PasswordHelper.HashPassword(company.Password);

            // Set default payment status
            company.PaymentStatus = "Pending";
            company.PaymentAmount = PaymentCalculator.GetCompanyAmount(company.PaymentType ?? "Monthly");

            // Save to database
            _context.Companies.Add(company);
            _context.SaveChanges();

            return RedirectToAction("Success", new { id = company.CompanyId });
        }


        public IActionResult Success(int id)
        {
            ViewData["CompanyId"] = id;
            return View();
        }


        //login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(CompanyLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var company = _context.Companies.FirstOrDefault(c =>
                c.CompanyUniqueId == model.CompanyUniqueId &&
                c.Password == PasswordHelper.HashPassword(model.Password));

            if (company == null)
            {
                ModelState.AddModelError("", "Invalid Company ID or Password.");
                return View(model);
            }

            // Set session
            HttpContext.Session.SetInt32("CompanyId", company.CompanyId);
            HttpContext.Session.SetString("CompanyName", company.CompanyName ?? "");

            return RedirectToAction("Dashboard");
        }


        public IActionResult Profile()
        {
            int? companyId = HttpContext.Session.GetInt32("CompanyId");
            if (companyId == null)
                return RedirectToAction("Login");

            var company = _context.Companies.Find(companyId);
            if (company == null)
                return RedirectToAction("Login");

            var model = new CompanyProfileViewModel
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyUniqueId = company.CompanyUniqueId,
                Email = company.Email,
                ContactPerson = company.ContactPerson,
                Designation = company.Designation,
                Mobile = company.Mobile,
                Telephone = company.Telephone,
                FaxNumber = company.FaxNumber,
                Address = company.Address,
                MembershipType = company.MembershipType,
                PaymentType = company.PaymentType,
                PaymentStatus = company.PaymentStatus,
                PaymentAmount = PaymentCalculator.GetCompanyAmount(company.PaymentType ?? "Monthly")
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(CompanyProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var company = _context.Companies.Find(model.CompanyId);
            if (company == null)
                return RedirectToAction("Login");

            // Update editable fields
            company.CompanyName = model.CompanyName;
            company.Email = model.Email;
            company.ContactPerson = model.ContactPerson;
            company.Designation = model.Designation;
            company.Mobile = model.Mobile;
            company.Telephone = model.Telephone;
            company.FaxNumber = model.FaxNumber;
            company.Address = model.Address;

            // Update membership & payment type
            company.MembershipType = model.MembershipType;
            company.PaymentType = model.PaymentType;

            // Reset payment status on type change
            company.PaymentStatus = "Pending";
            company.PaymentAmount = PaymentCalculator.GetCompanyAmount(model.PaymentType ?? "Monthly");

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Changes saved successfully!";


            // Update amount to display in View
            model.PaymentAmount = company.PaymentAmount;
            model.PaymentStatus = company.PaymentStatus;

            return View(model);
        }


        //dashboard
        public IActionResult Dashboard()
        {
            int? companyId = HttpContext.Session.GetInt32("CompanyId");
            if (companyId == null)
                return RedirectToAction("Login");

            var company = _context.Companies.Find(companyId.Value);
            if (company == null)
            {
                HttpContext.Session.Remove("CompanyId");
                HttpContext.Session.Remove("CompanyName");
                return RedirectToAction("Login");
            }

            return View(company);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CompanyId");
            HttpContext.Session.Remove("CompanyName");
            return RedirectToAction("Login");
        }


    }

}