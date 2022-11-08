using GrabDemSite.Data;
using GrabDemSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult Edit(string id)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            ViewBag.User = user;
            return View(user);
        }
        /* Tasks need to give money, based on a commision - 0.15%?
         * Level1 Users give 0.03% to the inviter
         * Level2 Users give 0.02% to the inviter
         * Level3 Users give 0.01% to the inviter
         */
        [HttpGet]
        public IActionResult Edit(string id, double balance)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            user.Balance += balance;
            user.MoneySpent += balance;
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult TryWithdraw(string id, double money)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            money -= money * 0.06;
            if (user.MoneySpent < 35)
            {
                ViewBag.ErrorBal = "You need to deposit at least 35$ in order to withdraw";
                return View("Withdraw", user);
            }
            else if(user.InviteCount<3)
            {
                ViewBag.ErrorRef = "You need to have invited at least 3 people who deposited at least 25$";
                return View("Withdraw", user);
            }
            else if (user.Balance < money)
            {
                ViewBag.ErrorNoMoney = "Your balance is less than what you want to withdraw";
                return View("Withdraw", user);
            }
            WithdrawDataModel withdrawReq = new WithdrawDataModel();
            withdrawReq.Id = Guid.NewGuid().ToString();
            withdrawReq.WalletAddress = user.WalletAddress;
            withdrawReq.Money = money;
            withdrawReq.User = user;
            return View("ConfirmWithdraw", withdrawReq);
        }
        public IActionResult ConfirmedWithdraw(string id, double money, string wallet, string iduser)
        {
            WithdrawDataModel withdrawReq = new WithdrawDataModel();
            withdrawReq.Id = id;
            withdrawReq.Money = money;
            withdrawReq.WalletAddress = wallet;
            withdrawReq.User = _context.Users.Where(x => x.Id == iduser).Single();
            _context.WithdrawDatas.Add(withdrawReq);
            _context.SaveChanges();
            return View("Index");
        }
        public IActionResult ChangeUser()
        {
            return RedirectToAction("AdminMenu", "Home");
        }
        public IActionResult Withdraw()
        {
            ViewBag.ErrorBal = "";
            ViewBag.ErrorRef = "";
            ViewBag.ErrorNoMoney = "";
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            return View(user);
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
};