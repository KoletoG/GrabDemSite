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
        readonly string Wallet = "xXXxxxXxxxxxXXxxX";
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult AdminMenu()
        {
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Orders = _context.DepositDatas.ToList(); 
            ViewBag.Withdraws = _context.WithdrawDatas.ToList();
            return View();
        }
        public IActionResult Edit(string id)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            ViewBag.Orders = _context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed==false).ToList();
            ViewBag.Withdraws = _context.WithdrawDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == false).ToList();

            return View(user);
        }
        /* Tasks need to give money, based on a commision - 0.15%?
         * Level1 Users give 0.03% to the inviter
         * Level2 Users give 0.02% to the inviter
         * Level3 Users give 0.01% to the inviter
         */ 
        public IActionResult AdminWithdrawConfirm(string wallet)
        {
            ViewBag.Withdraws = _context.WithdrawDatas.Where(x=>x.WalletAddress==wallet).ToList();
            
            return View();
        }
        public IActionResult ChangeWallet(string wallet)
        {
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            user.WalletAddress = wallet;
            _context.Update(user);
            _context.SaveChanges();
            return View("Index");
        }
        public IActionResult AdminWithdrawConfirmed(string id)
        {
            List<WithdrawDataModel> withdraws = _context.WithdrawDatas.Where(x => x.User.Id == id).ToList();
            for (int i=0;i< withdraws.Count();i++)
            {
                WithdrawDataModel withdraw = withdraws[i];
                withdraw.IsConfirmed = true;
                _context.Update(withdraw);
            }
            _context.SaveChanges();
            return RedirectToAction("AdminMenu", "Home");
        }
        [HttpGet]
        public IActionResult Edit(string id, double balance)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            user.Balance += balance;
            user.MoneySpent += balance;
            List<DepositDataModel> deposits = _context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == false).ToList();
           
            for (int i=0;i<deposits.Count();i++)
            {
                DepositDataModel deposit = deposits[i];
                deposit.IsConfirmed = true;
                _context.Update(deposit);
            }
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
            UserDataModel user = _context.Users.Where(x => x.Id == iduser).Single();
            withdrawReq.Id = id;
            withdrawReq.Money = money;
            withdrawReq.DateCreated = DateTime.Now;
            withdrawReq.IsConfirmed = false;
            withdrawReq.WalletAddress = "55555555555";
            withdrawReq.User = user;
            user.Balance -= money;
            _context.WithdrawDatas.Add(withdrawReq);
            _context.Update(user);
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
            ViewBag.ErrorSum = "";
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            return View(user);
        }
        public IActionResult TryDeposit(string id, double money)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            if (money <= 35)
            {
                ViewBag.ErrorSum = "The minimum amount for deposit is 35$";
                return View("Deposit", user);
            }
            else
            {
                DepositDataModel depReq = new DepositDataModel();
                depReq.MoneyForDeposit = money;
                depReq.User = user;
                depReq.Id = Guid.NewGuid().ToString();
                depReq.UserEmail = user.Email;
                ViewBag.Wallet = Wallet;
                return View("TryDeposit", depReq);
            }
        }
        public IActionResult TryTheDeposit(string id, double money, string userid)
        {
            UserDataModel user =_context.Users.Where(x=>x.Id == userid).Single();
            DepositDataModel deposit = new DepositDataModel();
            deposit.User = user;
            deposit.MoneyForDeposit = money;
            deposit.Id = id;
            deposit.IsConfirmed = false;
            deposit.DateCreated = DateTime.Now;
            deposit.UserEmail = user.Email;
            _context.DepositDatas.Add(deposit);
            _context.SaveChanges();
            return View("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
};