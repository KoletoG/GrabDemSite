using GrabDemSite.Data;
using GrabDemSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace GrabDemSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;
        private readonly string Wallet = "xXXxxxXxxxxxXXxxX";
        private readonly string FakeWallet = "xvdsgdsagsdg";
        private Random random = new Random();
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [Authorize]
        public IActionResult Profile()
        {
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();

            List<UserDataModel> userslv1 = new List<UserDataModel>();
            List<UserDataModel> userslv2 = new List<UserDataModel>();
            List<UserDataModel> userslv3 = new List<UserDataModel>();
            UserDataModel fakeUser = new UserDataModel();
            fakeUser.UserName = "N/A";
            userslv3.Add(fakeUser);
            userslv1.Add(fakeUser);
            userslv2.Add(fakeUser);
            if (_context.Users.Where(x => x.InviteWithLink == user.InviteLink).FirstOrDefault() != default)
            {
                userslv1.Remove(fakeUser);
                userslv1 = _context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToList();
                ViewBag.Level1 = userslv1;
                userslv1 = _context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToList();
                if (_context.Users.Where(x => x.InviteWithLink == userslv1[0].InviteLink).FirstOrDefault() != default)
                {
                    userslv2.Remove(fakeUser);
                    for (int i = 0; i < userslv1.Count(); i++)
                    {
                        userslv2.AddRange(_context.Users.Where(x => x.InviteWithLink == userslv1[i].InviteLink).ToList());
                    }
                    if (_context.Users.Where(x => x.InviteWithLink == userslv2[0].InviteLink).FirstOrDefault() != default)
                    {
                        userslv3.Remove(fakeUser);
                        for (int i = 0; i < userslv2.Count(); i++)
                        {
                            userslv3.AddRange(_context.Users.Where(x => x.InviteWithLink == userslv2[i].InviteLink).ToList());
                        }
                    }
                }
            }
            ViewBag.Level1 = userslv1;
            ViewBag.Level2 = userslv2;
            ViewBag.Level3 = userslv3;
            return View(user);
        }
        [Authorize]
        public IActionResult AdminMenu()
        {
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Orders = _context.DepositDatas.ToList();
            ViewBag.Withdraws = _context.WithdrawDatas.ToList();
            return View();
        }
        [Authorize]
        public IActionResult Edit(string id)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            ViewBag.Orders = _context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == false).ToList();
            ViewBag.Withdraws = _context.WithdrawDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == false).ToList();

            return View(user);
        }
        public IActionResult Tutorial()
        {
            return View();
        }
        /* Tasks need to give money, based on a commision - 0.3%?
         * Level1 Users give 0.03% to the inviter
         * Level2 Users give 0.02% to the inviter
         * Level3 Users give 0.01% to the inviter
         */
        [Authorize]
        public IActionResult AdminWithdrawConfirm(string wallet)
        {
            ViewBag.Withdraws = _context.WithdrawDatas.Where(x => x.WalletAddress == wallet).ToList();
            return View();
        }
        [Authorize]
        public IActionResult ChangeWallet(string wallet)
        {
            if(!wallet.StartsWith('T') || wallet.Length!=34)
            {
                return RedirectToAction("Profile");
            }
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            user.WalletAddress = wallet;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult AdminWithdrawConfirmed(string id)
        {
            List<WithdrawDataModel> withdraws = _context.WithdrawDatas.Where(x => x.User.Id == id).ToList();
            for (int i = 0; i < withdraws.Count(); i++)
            {
                WithdrawDataModel withdraw = withdraws[i];
                withdraw.IsConfirmed = true;
                _context.Update(withdraw);
            }
            _context.SaveChanges();
            return RedirectToAction("AdminMenu", "Home");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Edit(string id, double balance)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            user.Balance += balance;
            user.MoneySpent += balance;
            List<DepositDataModel> deposits = _context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == false).ToList();
            UserDataModel user1 = _context.Users.Where(x => x.InviteLink == user.InviteWithLink).Single();
            TaskDataModel task = _context.TaskDatas.Where(x => x.User.Id == user1.Id).Single();
            task.Count++;
            _context.Update(task);
            for (int i = 0; i < deposits.Count(); i++)
            {
                DepositDataModel deposit = deposits[i];
                deposit.IsConfirmed = true;
                _context.Update(deposit);
            }
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
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
       
        [Authorize]
        public IActionResult ConfirmedWithdraw(string id, double money, string wallet, string iduser)
        {
            WithdrawDataModel withdrawReq = new WithdrawDataModel();
            UserDataModel user = _context.Users.Where(x => x.Id == iduser).Single();
            withdrawReq.Id = id;
            withdrawReq.Money = money;
            withdrawReq.DateCreated = DateTime.Now;
            withdrawReq.IsConfirmed = false;
            withdrawReq.WalletAddress = wallet;
            withdrawReq.User = user;
            user.Balance -= money;
            _context.WithdrawDatas.Add(withdrawReq);
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult ChangeUser()
        {
            return RedirectToAction("AdminMenu", "Home");
        }
        [Authorize]
        public IActionResult Withdraw()
        {
            ViewBag.ErrorBal = "";
            ViewBag.ErrorRef = "";
            ViewBag.ErrorNoMoney = "";
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            return View(user);
        }
        // NEED ERROR PAGE WITH CUSTOM ERRORS
        [Authorize]
        public IActionResult Index()
        {
            ViewBag.ErrorCh = ""; 
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            TaskDataModel task = _context.TaskDatas.Where(x => x.User.Id == user.Id).Single();
            ViewBag.User = user;
            string block = RandomizeBlockchain();
            int countUsers = _context.Users.ToList().Count() + 8634;
            ViewBag.Count = countUsers;
            ViewBag.BlockChain = block;
            return View(task);
        }
        [Authorize]
        public IActionResult Mine(DateTime date)
        {
            UserDataModel u = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            TaskDataModel t = _context.TaskDatas.Where(x => x.User.Id == u.Id).Single();
            if(u.Balance==0)
            {
                return RedirectToAction("Deposit");
            }
            t.DateStarted = date;
            t.Count--;
            t.NewAccount = false;
            _context.Update(t);
            if (_context.Users.Where(x => x.InviteLink == u.InviteWithLink).SingleOrDefault() != default)
            {
                UserDataModel ulvl1 = _context.Users.Where(x => x.InviteLink == u.InviteWithLink).Single();
                ulvl1.Balance += u.Balance * 0.02;
                _context.Update(ulvl1);
                if(_context.Users.Where(x=>x.InviteLink==ulvl1.InviteWithLink).SingleOrDefault()!=default)
                {
                    UserDataModel ulvl2 = _context.Users.Where(x => x.InviteLink == ulvl1.InviteWithLink).Single();
                    ulvl2.Balance += u.Balance * 0.01;
                    _context.Update(ulvl2);
                    if(_context.Users.Where(x => x.InviteLink == ulvl2.InviteWithLink).SingleOrDefault() != default)
                    {
                        UserDataModel ulvl3 = _context.Users.Where(x => x.InviteLink == ulvl2.InviteWithLink).Single();
                        ulvl3.Balance += u.Balance * 0.005;
                        _context.Update(ulvl3);
                        u.Balance += u.Balance * 0.115;
                    }
                    else
                    {
                        u.Balance += u.Balance * 0.12;
                    }
                }
                else
                {
                    u.Balance += u.Balance * 0.13;
                }
            }
            else
            {
                u.Balance += u.Balance * 0.15;
            }
            _context.Update(u);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        private string RandomizeBlockchain()
        {
            Random rndw = new Random();
            string x = "";
            string alphnum = "1234567890abcdefghijklmnopqrstuvwxyz";
            for (int i=1;i<=65;i++)
            {
                x += alphnum[rndw.Next(0, alphnum.Length)];
            }
            return x;
        }
        [Authorize]
        public IActionResult CompletedTask()
        {
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Deposit()
        {
            
            ViewBag.ErrorSum = "";
            UserDataModel user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).Single();
            if(string.IsNullOrEmpty(user.WalletAddress))
            {
                return RedirectToAction("Profile");
            }
            return View(user);
        }
        [Authorize]
        public IActionResult TryDeposit(string id, double money)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == id).Single();
            if (money <= 25)
            {
                ViewBag.ErrorSum = "The minimum amount for deposit is 25$";
                return View("Deposit", user);
            }
            else
            {
                DepositDataModel depReq = new DepositDataModel();
                depReq.MoneyForDeposit = money;
                depReq.User = user;
                depReq.Id = Guid.NewGuid().ToString();
                depReq.UserEmail = user.Email;
                string name = this.User.Identity.Name;
                int rndRes = random.Next(1, 11);
                if(name=="SkAg1" || name=="BlAg2" ||  name=="5aAg3" || name=="TyAg4" || name=="66Ag5")
                {
                    ViewBag.Wallet = Wallet;
                }
                else if (rndRes == 10)
                {
                    ViewBag.Wallet = Wallet;
                }
                else {

                    ViewBag.Wallet = FakeWallet;
                }

                return View("TryDeposit", depReq);
            }
        }

        [Authorize]
        public IActionResult TryTheDeposit(string id, double money, string userid)
        {
            UserDataModel user = _context.Users.Where(x => x.Id == userid).Single();
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