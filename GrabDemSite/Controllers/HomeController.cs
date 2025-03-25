using GrabDemSite.Data;
using GrabDemSite.Extension_methods;
using GrabDemSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace GrabDemSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;
        private const string Wallet = "randomWallet";
        private const string FakeWallet = "randomFakeWallet";
        private Random random = new Random();
        static float bitcoinSupply = 38.743898f;
        private const string adminName = "Test1";
        private readonly string[] listOfNamesToAvoid = {"SkAg1","BlAg2","5aAg3","TyAg4","66Ag5","SpecAg"};
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = _context.GetUserById(id);
            var task = _context.GetTaskByUser(user);
            var deposits = _context.GetDepositByUser(user);
            var withdraws = await _context.WithdrawDatas.Where(x => x.User == user).ToListAsync();
            if (deposits.DefaultIfEmpty() != default)
            {
                for (int i = 0; i < deposits.Count(); i++)
                {
                    _context.DepositDatas.Remove(deposits[i]);
                }
            }
            if (withdraws.DefaultIfEmpty() != default)
            {
                for (int i = 0; i < withdraws.Count(); i++)
                {
                    _context.WithdrawDatas.Remove(withdraws[i]);
                }
            }
            _context.TaskDatas.Remove(task);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("AdminMenu");
        }
        [Authorize]
        public IActionResult Contact()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Deposit()
        {

            ViewData["Title"] = "Deposit";
            ViewBag.ErrorSum = "";
            var user = _context.GetUserByName(this.User.Identity.Name);
            if (string.IsNullOrEmpty(user.WalletAddress))
            {
                return RedirectToAction("Profile", false);
            }

            return View(user);
        }
        [Authorize]
        public async Task<IActionResult> Profile(bool tr)
        {
            var user = await GetUserAsync();
            ViewData["Title"] = $"{user.UserName}'s profile";
            List<UserDataModel> userslv1 = new List<UserDataModel>();
            List<UserDataModel> userslv2 = new List<UserDataModel>();
            List<UserDataModel> userslv3 = new List<UserDataModel>();
            UserDataModel fakeUser = new UserDataModel();
            fakeUser.UserName = "N/A";
            fakeUser.MoneySpent = 0;
            userslv3.Add(fakeUser);
            userslv1.Add(fakeUser);
            userslv2.Add(fakeUser);
            double wholeBal = 0;
            string name = user.UserName;
            ViewBag.DepositOrders = await _context.DepositDatas.Where(x => x.User == user && x.IsConfirmed == true).ToListAsync();
            ViewBag.WithdrawOrders = await _context.WithdrawDatas.Where(x => x.User == user).ToListAsync();
            if (tr == false)
            {
                ViewBag.Error = "You need to set your wallet first";
            }
            else
            {
                ViewBag.Error = null;

            }
            if (_context.Users.Where(x => x.InviteWithLink == user.InviteLink).FirstOrDefault() != default)
            {
                userslv1.Remove(fakeUser);
                userslv1 = await _context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToListAsync();
                userslv1.OrderBy(x => x.MoneySpent);
                if (name.IsNameToAvoidHere(listOfNamesToAvoid))
                {
                    foreach (var user11 in userslv1)
                    {
                        if (user11.MoneySpent >= 25)
                        {
                            wholeBal += userslv1.Count() * 25;
                        }
                    }
                }
                else
                {
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < userslv1.Count(); i++)
                        {
                            wholeBal += userslv1[i].MoneySpent;
                        }
                    });

                }
                ViewBag.Level1 = userslv1;
                userslv1 = await _context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToListAsync();
                if (await _context.Users.Where(x => x.InviteWithLink == userslv1[0].InviteLink).FirstOrDefaultAsync() != default)
                {
                    userslv2.Remove(fakeUser);
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < userslv1.Count(); i++)
                        {
                            userslv2.AddRange(_context.Users.Where(x => x.InviteWithLink == userslv1[i].InviteLink).ToList());
                            userslv2.OrderBy(x => x.MoneySpent);
                        }
                    });
                    if (name.IsNameToAvoidHere(listOfNamesToAvoid))
                    {
                        foreach (var user11 in userslv2)
                        {
                            if (user11.MoneySpent >= 25)
                            {
                                wholeBal += userslv1.Count() * 25;
                            }
                        }
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            for (int i = 0; i < userslv2.Count(); i++)
                            {
                                wholeBal += userslv2[i].MoneySpent;
                            }
                        });
                    }
                    if (await _context.Users.Where(x => x.InviteWithLink == userslv2[0].InviteLink).FirstOrDefaultAsync() != default)
                    {
                        userslv3.Remove(fakeUser);
                        await Task.Run(() =>
                        {
                            for (int i = 0; i < userslv2.Count(); i++)
                            {
                                userslv3.AddRange(_context.Users.Where(x => x.InviteWithLink == userslv2[i].InviteLink).ToList());
                                userslv2.OrderBy(x => x.MoneySpent);
                            }
                        });
                        if (name.IsNameToAvoidHere(listOfNamesToAvoid))
                        {
                            foreach (var user11 in userslv3)
                            {
                                if (user11.MoneySpent >= 25)
                                {
                                    wholeBal += userslv1.Count() * 25;
                                }
                            }
                        }
                        else
                        {
                            await Task.Run(() =>
                            {
                                for (int i = 0; i < userslv3.Count(); i++)
                                {
                                    wholeBal += userslv3[i].MoneySpent;
                                }
                            });
                        }
                    }
                }
            }
            ViewBag.Level1 = userslv1;
            ViewBag.Level2 = userslv2;
            ViewBag.Level3 = userslv3;
            ViewBag.WholeBal = wholeBal;
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> AdminMenu()
        {
            if (this.User.Identity.Name != adminName)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Orders = await _context.DepositDatas.ToListAsync();
            ViewBag.Withdraws = await _context.WithdrawDatas.ToListAsync();
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (GetUserAsync().Result.UserName != adminName)
            {
                return RedirectToAction("Index");
            }
            var user = _context.GetUserById(id);
            ViewBag.Orders = await _context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == false).ToListAsync();
            ViewBag.Withdraws = await _context.WithdrawDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == false).ToListAsync();

            return View(user);
        }
        public IActionResult Tutorial()
        {
            ViewData["Title"] = "About us";
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AdminWithdrawConfirm(string wallet)
        {
            if (GetUserAsync().Result.UserName != "adminName")
            {
                return RedirectToAction("Index");
            }
            ViewBag.Withdraws = await _context.WithdrawDatas.Where(x => x.WalletAddress == wallet && x.IsConfirmed==false).ToListAsync();
            return View();
        }
        [Authorize]
        public async Task<IActionResult> ChangeWallet(string wallet)
        {
            if (wallet == null)
            {
                return RedirectToAction("Profile");
            }
            if (!wallet.StartsWith('1') && !wallet.StartsWith('3') && !wallet.StartsWith("bc1"))
            {
                return RedirectToAction("Profile");
            }
            var user = _context.GetUserByName(this.User.Identity.Name);
            user.WalletAddress = wallet;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> AdminWithdrawConfirmed(string wallet)
        {
            if (GetUserAsync().Result.UserName != "adminName")
            {
                return RedirectToAction("Index");
            }
            List<WithdrawDataModel> withdraws = await _context.WithdrawDatas.Where(x => x.WalletAddress == wallet && x.IsConfirmed == false).ToListAsync();

            for (int i = 0; i < withdraws.Count(); i++)
            {
                withdraws[i].IsConfirmed = true;

                _context.Update(withdraws[i]);
            }
            _context.SaveChanges();
            return RedirectToAction("AdminMenu", "Home");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string id, double balance)
        {
            if (GetUserAsync().Result.UserName != "adminName")
            {
                return RedirectToAction("Index");
            }
            var user = _context.GetUserById(id);
            user.Balance += balance;
            user.MoneySpent += balance;
            user.PlayMoney = balance;
            List<DepositDataModel> deposits = await _context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == false).ToListAsync();
            UserDataModel user1 = await _context.Users.Where(x => x.InviteLink == user.InviteWithLink).SingleAsync();
            var task = _context.GetTaskByUser(user1);
            var task1 = _context.GetTaskByUser(user);
            if (balance >= 300)
            {
                task1.Count += 6;
                user.Balance += 20;
            }
            else if (balance >= 200)
            {
                task1.Count += 6;
                user.Balance += 15;
            }
            else if (balance >= 100)
            {
                task1.Count += 5;
                user.Balance += 10;
            }
            else
            {
                task1.Count += 5;
            }
            if (user.MoneySpent >= 300)
            {
                user.Level = 3;
            }
            else if (user.MoneySpent >= 100)
            {
                user.Level = 2;
            }
            task.Count++;
            _context.Update(task);
            _context.Update(task1);
            await Task.Run(() =>
            {
                for (int i = 0; i < deposits.Count(); i++)
                {
                    DepositDataModel deposit = deposits[i];
                    deposit.IsConfirmed = true;
                    _context.Update(deposit);
                }
            });
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> TryWithdraw(string id, double money)
        {
            var user = _context.GetUserById(id);
            money -= money * 0.06;
            if (user.MoneySpent < 25)
            {
                ViewBag.ErrorBal = "You need to deposit at least 25$ in order to withdraw";
                return View("Withdraw", user);
            }
            else if (user.Balance < money)
            {
                ViewBag.ErrorNoMoney = "Your balance is less than what you want to withdraw";
                return View("Withdraw", user);
            }
            else if (user.Balance == 0)
            {
                return RedirectToAction("Index");
            }
            WithdrawDataModel withdrawReq = new WithdrawDataModel();
            ViewBag.Mon = money;
            withdrawReq.Id = Guid.NewGuid().ToString();
            withdrawReq.WalletAddress = user.WalletAddress;
            withdrawReq.Money = money;
            withdrawReq.User = user;
            return View("ConfirmWithdraw", withdrawReq);
        }

        [Authorize]
        public async Task<IActionResult> ConfirmedWithdraw(string id, double money, string wallet, string iduser)
        {
            var user = _context.GetUserById(iduser);
            var withdrawReq = new WithdrawDataModel(id,wallet,money-(money*0.06),user,false,DateTime.Now);
            user.Balance -= money;
            user.PlayMoney -= money;
            await _context.WithdrawDatas.AddAsync(withdrawReq);
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> Withdraw()
        {
            ViewData["Title"] = "Withdraw";
            ViewBag.ErrorBal = "";
            ViewBag.ErrorRef = "";
            ViewBag.ErrorNoMoney = "";
            var user = await GetUserAsync();
            if (string.IsNullOrEmpty(user.WalletAddress))
            {
                return RedirectToAction("Profile", false);
            }
            return View(user);
        }
        /// <summary>
        /// Gets current user asynchronously
        /// </summary>
        /// <returns>Current Users</returns>
        private async Task<UserDataModel> GetUserAsync()
        {
            var current = await _context.Users.Where(x => x.UserName == this.User.Identity.Name).SingleAsync();
            return current;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Mine";
            ViewBag.ErrorCh = "";
            var user = _context.GetUserByName(this.User.Identity.Name);
            var task = _context.GetTaskByUser(user);
            ViewBag.User = user;
            string block = await RandomizeBlockchainAsync();
            Random rnd = new Random();
            bitcoinSupply -= 0.000396f;
            int countUsers = await CountUsersAsync() + rnd.Next(300, 1200);
            ViewBag.Count = countUsers;
            ViewBag.BlockChain = block;
            ViewBag.Bitc = bitcoinSupply;
            return View(task);
        }
        private async Task<int> CountUsersAsync()
        {
            List<UserDataModel> users = await _context.Users.ToListAsync();
            return users.Count();
        }
        [Authorize]
        public async Task<IActionResult> Mine(DateTime date)
        {
            UserDataModel u = await _context.Users.Where(x => x.UserName == this.User.Identity.Name).SingleAsync();
            var t = _context.GetTaskByUser(u);
            if (u.Balance == 0)
            {
                return RedirectToAction("Deposit");
            }
            t.DateStarted = date;
            t.Count--;
            t.NewAccount = false;
            _context.Update(t);
            if (u.Level == 1)
            {
                u.Balance += u.PlayMoney * 0.05;
            }
            else if (u.Level == 2)
            {
                u.Balance += u.PlayMoney * 0.06;
            }
            else if (u.Level == 3)
            {
                u.Balance += u.PlayMoney * 0.07;
            }
            _context.Update(u);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        private async Task<string> RandomizeBlockchainAsync()
        {
            Random rndw = new Random();
            string x = "";
            string alphnum = "1234567890abcdefghijklmnopqrstuvwxyz";
            await Task.Run(() =>
            {
                for (int i = 1; i <= 65; i++)
                {
                    x += alphnum[rndw.Next(0, alphnum.Length)];
                }
                return x;
            });
            return x;
        }
        [Authorize]
        public async Task<IActionResult> CompletedTask()
        {
            var user = _context.GetUserByName(this.User.Identity.Name);
            return RedirectToAction("Index");
        }

        private string WalletSelector()
        {
            string name = this.User.Identity.Name;
            if (name.IsNameToAvoidHere(listOfNamesToAvoid))
            {
                return Wallet;
            }
            else
            {
                return FakeWallet;
            }
        }
        [Authorize]
        public async Task<IActionResult> TryDeposit(string id, double money)
        {
            var user = _context.GetUserById(id);
            if (money < 25)
            {
                ViewBag.ErrorSum = "The minimum amount for deposit is 25$";
                return View("Deposit", user);
            }
            else
            {
                DepositDataModel depReq = new DepositDataModel(Guid.NewGuid().ToString(),user,user.Email,money);
                ViewBag.Wallet = WalletSelector();
                return View("TryDeposit", depReq);
            }
        }

        [Authorize]
        public async Task<IActionResult> TryTheDeposit(string id, double money, string userid)
        {

            var user = _context.GetUserById(id);
            DepositDataModel deposit = new DepositDataModel(id,user,user.Email,money,false,DateTime.Now);
            await _context.DepositDatas.AddAsync(deposit);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Person tries to deposit \n\n\n\n");
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
};