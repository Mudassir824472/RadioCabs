using Microsoft.AspNetCore.Mvc;
using RadioCabs.Models;
using RadioCabs.Helpers;


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

            // Save to database
            _context.Companies.Add(company);
            _context.SaveChanges();

            return RedirectToAction("Success");
        }


        public IActionResult Success()
        {
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

            string hashedPassword = PasswordHelper.HashPassword(model.Password);

            var company = _context.Companies.FirstOrDefault(c =>
                c.CompanyUniqueId == model.CompanyUniqueId &&
                c.Password == hashedPassword);

            if (company == null)
            {
                ModelState.AddModelError("", "Invalid Company ID or Password");
                return View(model);
            }

            // ✅ SET SESSION
            HttpContext.Session.SetInt32("CompanyId", company.CompanyId);
            HttpContext.Session.SetString("CompanyName", company.CompanyName);

            return RedirectToAction("Dashboard");
        }

        //logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        //prfile
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
                PaymentAmount = CalculatePaymentAmount(company.PaymentType, company.MembershipType)
            };

            return View(model);
        }
        private int CalculatePaymentAmount(string paymentType, string membershipType)
        {
            // Rule 1: Free membership pays nothing
            if (membershipType == "Free")
            {
                return 0;
            }

            // Rule 2: Paid memberships
            return paymentType switch
            {
                "Monthly" => 15,
                "Quarterly" => 40,
                _ => 0
            };
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

            // Membership & payment
            company.MembershipType = model.MembershipType;
            company.PaymentType = model.PaymentType;

            // 🔑 Calculate amount FIRST
            int amount = CalculatePaymentAmount(
                model.PaymentType,
                model.MembershipType
            );

            company.PaymentAmount = amount;

            // 🔑 Set status correctly
            company.PaymentStatus = amount == 0 ? "Free" : "Pending";

            // Save everything together
            _context.SaveChanges();

            // Sync ViewModel
            model.PaymentAmount = (int)company.PaymentAmount;
            model.PaymentStatus = company.PaymentStatus;

            return View(model);
        }






        //dahsboard
        public IActionResult Dashboard()
        {
            return View();
        }


    }

}
