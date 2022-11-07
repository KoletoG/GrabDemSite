using GrabDemSite.Data;
using GrabDemSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GrabDemSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult AdminMenu()
        {
            ViewBag.Users = _context.Users.ToList();

            return View();
        }
        public IActionResult ChangeUser()
        {
            return RedirectToAction("AdminMenu", "Home");
        }
        public IActionResult Withdraw()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Deposit()
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