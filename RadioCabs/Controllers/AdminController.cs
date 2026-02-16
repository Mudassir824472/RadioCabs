using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RadioCabs.Helpers;
using RadioCabs.Models;
using System.Linq;

namespace RadioCabs.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(ApplicationContext context, IConfiguration configuration)
            {
            _context = context;
            _configuration = configuration;
        }

        // ==================== AUTHENTICATION ====================

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewData["Error"] = "Username and password are required.";
                return View();
            }
            var normalizedUsername = username.Trim();
            var passwordHash = PasswordHelper.HashPassword(password);

            var adminUser = _context.AdminUsers.FirstOrDefault(a => a.Username == normalizedUsername && a.IsActive);
            if (adminUser != null)
            {
                if (adminUser.PasswordHash != passwordHash)
                {
                    ViewData["Error"] = "Invalid admin credentials.";
                    return View();
                }
                HttpContext.Session.SetString("IsAdmin", "true");
                HttpContext.Session.SetString("AdminUsername", username);
                HttpContext.Session.SetString("AdminUsername", adminUser.Username);
                return RedirectToAction(nameof(Dashboard));
            }
            if (!_context.AdminUsers.Any())
            {
                var bootstrapUsername = _configuration["AdminBootstrap:Username"];
                var bootstrapPasswordHash = _configuration["AdminBootstrap:PasswordHash"];

                if (!string.IsNullOrWhiteSpace(bootstrapUsername) &&
                    !string.IsNullOrWhiteSpace(bootstrapPasswordHash) &&
                    normalizedUsername == bootstrapUsername &&
                    passwordHash == bootstrapPasswordHash)
                {
                    var bootstrapAdmin = new AdminUser
                    {
                        Username = normalizedUsername,
                        PasswordHash = bootstrapPasswordHash,
                        IsActive = true
                    };

                    _context.AdminUsers.Add(bootstrapAdmin);
                    _context.SaveChanges();

                    HttpContext.Session.SetString("IsAdmin", "true");
                    HttpContext.Session.SetString("AdminUsername", bootstrapAdmin.Username);
                    return RedirectToAction(nameof(Dashboard));
                }
            }

            ViewData["Error"] = "Invalid admin credentials.";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            HttpContext.Session.Remove("AdminUsername");
            return RedirectToAction(nameof(Login));
        }

        // ==================== DASHBOARD ====================

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction(nameof(Login));
            }

            var companies = _context.Companies.ToList();
            var drivers = _context.Drivers.ToList();
            var advertisements = _context.Advertisements.ToList();
            var feedbacks = _context.Feedbacks.ToList();

            string NormalizeStatus(string? status)
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    return "Pending";
                }

                return status.Trim().Equals("Paid", System.StringComparison.OrdinalIgnoreCase)
                    ? "Paid"
                    : "Pending";
            }

            string NormalizeMembership(string? membershipType)
            {
                if (string.IsNullOrWhiteSpace(membershipType))
                {
                    return "Free";
                }

                var normalized = membershipType.Trim();
                if (normalized.Equals("Premium", System.StringComparison.OrdinalIgnoreCase))
                {
                    return "Premium";
                }

                if (normalized.Equals("Basic", System.StringComparison.OrdinalIgnoreCase))
                {
                    return "Basic";
                }

                return "Free";
            }

            var paidCompanyRevenue = companies.Where(c => NormalizeStatus(c.PaymentStatus) == "Paid").Sum(c => c.PaymentAmount);
            var paidDriverRevenue = drivers.Where(d => NormalizeStatus(d.PaymentStatus) == "Paid").Sum(d => d.PaymentAmount);
            var paidAdvertisementRevenue = advertisements.Where(a => NormalizeStatus(a.PaymentStatus) == "Paid").Sum(a => a.PaymentAmount);




            var model = new AdminDashboardViewModel
            {
                Companies = companies.OrderByDescending(c => c.CompanyId).ToList(),
                Drivers = drivers.OrderByDescending(d => d.DriverId).ToList(),
                Advertisements = advertisements.OrderByDescending(a => a.AdvertisementId).ToList(),
                Feedbacks = feedbacks.OrderByDescending(f => f.FeedbackId).ToList(),

                // Statistics
                TotalCompanies = companies.Count,
                TotalDrivers = drivers.Count,
                TotalAdvertisements = advertisements.Count,
                TotalFeedbacks = feedbacks.Count,

                // Payment Statistics
                PendingPayments = companies.Count(c => NormalizeStatus(c.PaymentStatus) == "Pending") +
                                 drivers.Count(d => NormalizeStatus(d.PaymentStatus) == "Pending") +
                                 advertisements.Count(a => NormalizeStatus(a.PaymentStatus) == "Pending"),
                CompletedPayments = companies.Count(c => NormalizeStatus(c.PaymentStatus) == "Paid") +
                                   drivers.Count(d => NormalizeStatus(d.PaymentStatus) == "Paid") +
                                   advertisements.Count(a => NormalizeStatus(a.PaymentStatus) == "Paid"),
                TotalRevenue = paidCompanyRevenue + paidDriverRevenue + paidAdvertisementRevenue,

                // Membership Statistics
                PremiumMembers = companies.Count(c => NormalizeMembership(c.MembershipType) == "Premium"),
                BasicMembers = companies.Count(c => NormalizeMembership(c.MembershipType) == "Basic"),
                FreeMembers = companies.Count(c => NormalizeMembership(c.MembershipType) == "Free")
            };


            return View(model);
        }

        // ==================== COMPANY MANAGEMENT ====================

        [HttpGet]
        public IActionResult ManageCompanies()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var companies = _context.Companies.OrderByDescending(c => c.CompanyId).ToList();
            return View(companies);
        }

        [HttpGet]
        public IActionResult ViewCompanyDetails(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var company = _context.Companies.Find(id);
            if (company == null)
                return NotFound();

            return View(company);
        }

        [HttpGet]
        public IActionResult EditCompany(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var company = _context.Companies.Find(id);
            if (company == null)
                return NotFound();

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCompany(int id, Company company)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            if (id != company.CompanyId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(company);

            var existingCompany = _context.Companies.Find(id);
            if (existingCompany == null)
                return NotFound();

            // Update fields
            existingCompany.CompanyName = company.CompanyName;
            existingCompany.Email = company.Email;
            existingCompany.ContactPerson = company.ContactPerson;
            existingCompany.Designation = company.Designation;
            existingCompany.Mobile = company.Mobile;
            existingCompany.Telephone = company.Telephone;
            existingCompany.FaxNumber = company.FaxNumber;
            existingCompany.Address = company.Address;
            existingCompany.MembershipType = company.MembershipType;
            existingCompany.PaymentType = company.PaymentType;
            existingCompany.PaymentStatus = company.PaymentStatus;

            _context.SaveChanges();
            TempData["Success"] = "Company updated successfully.";
            return RedirectToAction(nameof(ManageCompanies));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCompany(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var company = _context.Companies.Find(id);
            if (company == null)
                return NotFound();

            _context.Companies.Remove(company);
            _context.SaveChanges();
            TempData["Success"] = "Company deleted successfully.";
            return RedirectToAction(nameof(ManageCompanies));
        }

        // ==================== DRIVER MANAGEMENT ====================

        [HttpGet]
        public IActionResult ManageDrivers()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var drivers = _context.Drivers.OrderByDescending(d => d.DriverId).ToList();
            return View(drivers);
        }

        [HttpGet]
        public IActionResult ViewDriverDetails(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var driver = _context.Drivers.Find(id);
            if (driver == null)
                return NotFound();

            return View(driver);
        }

        [HttpGet]
        public IActionResult EditDriver(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var driver = _context.Drivers.Find(id);
            if (driver == null)
                return NotFound();

            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDriver(int id, Driver driver)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            if (id != driver.DriverId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(driver);

            var existingDriver = _context.Drivers.Find(id);
            if (existingDriver == null)
                return NotFound();

            // Update fields
            existingDriver.DriverName = driver.DriverName;
            existingDriver.Email = driver.Email;
            existingDriver.Mobile = driver.Mobile;
            existingDriver.Telephone = driver.Telephone;
            existingDriver.Address = driver.Address;
            existingDriver.City = driver.City;
            existingDriver.Experience = driver.Experience;
            existingDriver.Description = driver.Description;
            existingDriver.PaymentType = driver.PaymentType;
            existingDriver.PaymentStatus = driver.PaymentStatus;

            _context.SaveChanges();
            TempData["Success"] = "Driver updated successfully.";
            return RedirectToAction(nameof(ManageDrivers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDriver(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var driver = _context.Drivers.Find(id);
            if (driver == null)
                return NotFound();

            _context.Drivers.Remove(driver);
            _context.SaveChanges();
            TempData["Success"] = "Driver deleted successfully.";
            return RedirectToAction(nameof(ManageDrivers));
        }

        // ==================== ADVERTISEMENT MANAGEMENT ====================

        [HttpGet]
        public IActionResult ManageAdvertisements()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var advertisements = _context.Advertisements.OrderByDescending(a => a.AdvertisementId).ToList();
            return View(advertisements);
        }

        [HttpGet]
        public IActionResult ViewAdvertisementDetails(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var advertisement = _context.Advertisements.Find(id);
            if (advertisement == null)
                return NotFound();

            return View(advertisement);
        }

        [HttpGet]
        public IActionResult EditAdvertisement(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var advertisement = _context.Advertisements.Find(id);
            if (advertisement == null)
                return NotFound();

            return View(advertisement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAdvertisement(int id, Advertisement advertisement)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            if (id != advertisement.AdvertisementId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(advertisement);

            var existingAdvertisement = _context.Advertisements.Find(id);
            if (existingAdvertisement == null)
                return NotFound();

            // Update fields
            existingAdvertisement.CompanyName = advertisement.CompanyName;
            existingAdvertisement.Email = advertisement.Email;
            existingAdvertisement.Mobile = advertisement.Mobile;
            existingAdvertisement.Telephone = advertisement.Telephone;
            existingAdvertisement.FaxNumber = advertisement.FaxNumber;
            existingAdvertisement.Description = advertisement.Description;
            existingAdvertisement.PaymentType = advertisement.PaymentType;
            existingAdvertisement.PaymentStatus = advertisement.PaymentStatus;

            _context.SaveChanges();
            TempData["Success"] = "Advertisement updated successfully.";
            return RedirectToAction(nameof(ManageAdvertisements));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAdvertisement(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var advertisement = _context.Advertisements.Find(id);
            if (advertisement == null)
                return NotFound();

            _context.Advertisements.Remove(advertisement);
            _context.SaveChanges();
            TempData["Success"] = "Advertisement deleted successfully.";
            return RedirectToAction(nameof(ManageAdvertisements));
        }

        // ==================== FEEDBACK MANAGEMENT ====================

        [HttpGet]
        public IActionResult ManageFeedbacks()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var feedbacks = _context.Feedbacks.OrderByDescending(f => f.FeedbackId).ToList();
            return View(feedbacks);
        }

        [HttpGet]
        public IActionResult ViewFeedbackDetails(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var feedback = _context.Feedbacks.Find(id);
            if (feedback == null)
                return NotFound();

            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFeedback(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var feedback = _context.Feedbacks.Find(id);
            if (feedback == null)
                return NotFound();

            _context.Feedbacks.Remove(feedback);
            _context.SaveChanges();
            TempData["Success"] = "Feedback deleted successfully.";
            return RedirectToAction(nameof(ManageFeedbacks));
        }

        // ==================== PAYMENT MANAGEMENT ====================

        [HttpGet]
        public IActionResult ManagePayments()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            var paymentViewModel = new PaymentManagementViewModel
            {
                CompanyPayments = _context.Companies.OrderByDescending(c => c.CompanyId).ToList(),
                DriverPayments = _context.Drivers.OrderByDescending(d => d.DriverId).ToList(),
                AdvertisementPayments = _context.Advertisements.OrderByDescending(a => a.AdvertisementId).ToList()
            };

            return View(paymentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePaymentStatus(string entityType, int entityId, string paymentStatus)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction(nameof(Login));

            switch (entityType.ToLower())
            {
                case "company":
                    var company = _context.Companies.Find(entityId);
                    if (company != null)
                    {
                        company.PaymentStatus = paymentStatus;
                        _context.SaveChanges();
                    }
                    break;

                case "driver":
                    var driver = _context.Drivers.Find(entityId);
                    if (driver != null)
                    {
                        driver.PaymentStatus = paymentStatus;
                        _context.SaveChanges();
                    }
                    break;

                case "advertisement":
                    var advertisement = _context.Advertisements.Find(entityId);
                    if (advertisement != null)
                    {
                        advertisement.PaymentStatus = paymentStatus;
                        _context.SaveChanges();
                    }
                    break;
            }

            TempData["Success"] = "Payment status updated successfully.";
            return RedirectToAction(nameof(ManagePayments));
        }
    }

    // Helper ViewModel for Payment Management
    public class PaymentManagementViewModel
    {
        public List<Company> CompanyPayments { get; set; } = new List<Company>();
        public List<Driver> DriverPayments { get; set; } = new List<Driver>();
        public List<Advertisement> AdvertisementPayments { get; set; } = new List<Advertisement>();
    }
}