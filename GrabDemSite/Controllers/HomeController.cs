using GrabDemSite.Data;
using GrabDemSite.Extension_methods;
using GrabDemSite.Models;
using GrabDemSite.Static_Methods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GrabDemSite.Methods;
namespace GrabDemSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private const string Wallet = "randomWallet";
        private const string FakeWallet = "randomFakeWallet";
        private static float bitcoinSupply = 38.743898f;
        private const string adminName = "Test1";
        private readonly string[] listOfNamesToAvoid = { "SkAg1", "BlAg2", "5aAg3", "TyAg4", "66Ag5", "SpecAg" };
        private readonly Random rnd = new();
        private readonly MethodsCall methods;
        private readonly string userName;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            methods = new MethodsCall(_context, this, Wallet, FakeWallet, listOfNamesToAvoid, rnd);
            userName = this.User.Identity?.Name ?? "N/A";
        }
        /// <summary>
        /// Deletes a user's account
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <returns>Admin page</returns>
        [Authorize]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await _context.GetUserByIdAsync(id);
            var task = await _context.GetTaskByUserAsync(user);
            var deposits = await _context.GetDepositsByUserAsync(user);
            var withdraws = await _context.GetWithdrawsByUserAsync(user);
            if (deposits.DefaultIfEmpty() != default)
            {
                _context.DepositDatas.RemoveRange(deposits);
            }
            if (withdraws.DefaultIfEmpty() != default)
            {
                _context.WithdrawDatas.RemoveRange(withdraws);
            }
            _context.TaskDatas.Remove(task);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminMenu");
        }
        /// <summary>
        /// Contact page
        /// </summary>
        /// <returns>Contact page</returns>
        [Authorize]
        public IActionResult Contact()
        {
            return View();
        }
        /// <summary>
        /// Redirects user to deposit page
        /// </summary>
        /// <returns>Deposit page</returns>
        [Authorize]
        public async Task<IActionResult> Deposit()
        {
            ViewData["Title"] = "Deposit";
            ViewBag.ErrorSum = "";
            var user = await _context.GetUserByNameAsync(userName);
            if (string.IsNullOrEmpty(user.WalletAddress))
            {
                return RedirectToAction("Profile", false);
            }
            return View(user);
        }
        /// <summary>
        /// Shows your profile information
        /// </summary>
        /// <param name="tr">checks if wallet is set</param>
        /// <returns>Profile page</returns>
        [Authorize]
        public async Task<IActionResult> Profile(bool tr)
        {
            var user = await methods.GetUserAsync();
            ViewData["Title"] = $"{user.UserName}'s profile";
            var userslv1 = new List<UserDataModel>();
            var userslv2 = new List<UserDataModel>();
            var userslv3 = new List<UserDataModel>();
            var fakeUser = new UserDataModel();
            fakeUser.UserName = "N/A";
            fakeUser.MoneySpent = 0;
            userslv3.Add(fakeUser);
            userslv1.Add(fakeUser);
            userslv2.Add(fakeUser);
            double wholeBal = 0;
            string name = user.UserName;
            ViewBag.DepositOrders = await _context.GetDepositsByUserAndIsConfirmedAsync(user);
            ViewBag.WithdrawOrders = await _context.GetWithdrawsByUserAsync(user);
            if (tr == false)
            {
                ViewBag.Error = "You need to set your wallet first";
            }
            else
            {
                ViewBag.Error = null;
            }
            if (await _context.IsInviteLinkUsersExistAsync(user))
            {
                await _context.AddUsersToTeamByLevelAsync(userslv1, user, fakeUser);
                if (listOfNamesToAvoid.Contains(name))
                {
                    StaticWorkMethods.AddBalanceByUserCount(ref wholeBal, userslv1);
                }
                else
                {
                    StaticWorkMethods.AddBalanceByUserMoney(ref wholeBal, userslv1);
                }
                ViewBag.Level1 = userslv1;
                if (await _context.IsInviteLinkUsersExistAsync(userslv1))
                {
                    await _context.AddUsersToTeamByLevelAsync(userslv1, userslv2, fakeUser);
                    if (listOfNamesToAvoid.Contains(name))
                    {
                        StaticWorkMethods.AddBalanceByUserCount(ref wholeBal, userslv2);
                    }
                    else
                    {
                        StaticWorkMethods.AddBalanceByUserMoney(ref wholeBal, userslv2);
                    }
                    if (await _context.IsInviteLinkUsersExistAsync(userslv2))
                    {
                        await _context.AddUsersToTeamByLevelAsync(userslv2, userslv3, fakeUser);
                        if (listOfNamesToAvoid.Contains(name))
                        {
                            StaticWorkMethods.AddBalanceByUserCount(ref wholeBal, userslv3);
                        }
                        else
                        {
                            StaticWorkMethods.AddBalanceByUserMoney(ref wholeBal, userslv3);
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
        /// <summary>
        /// Shows every user and their withdraws and deposits
        /// </summary>
        /// <returns>Page with all important information about users</returns>
        [Authorize]
        public async Task<IActionResult> AdminMenu()
        {
            if (userName != adminName)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Users = await _context.LoadViewBagAllAsync<UserDataModel>();
            ViewBag.Orders = await _context.LoadViewBagAllAsync<DepositDataModel>();
            ViewBag.Orders = await _context.LoadViewBagAllAsync<WithdrawDataModel>();
            return View();
        }
        /// <summary>
        /// Views unconfirmed deposits and withdraws / Admin only
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <returns>Page with unconfirmed deposits and withdraws</returns>
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if ((await methods.GetUserAsync()).UserName != adminName)
            {
                return RedirectToAction("Index");
            }
            var user = await _context.GetUserByIdAsync(id);
            ViewBag.Orders = await _context.GetDepositsByUserIdAndIsConfirmedAsync(user, false);
            ViewBag.Withdraws = await _context.GetWithdrawsByUserIdAndIsConfirmedAsync(user, false);

            return View(user);
        }
        /// <summary>
        /// About us section
        /// </summary>
        /// <returns>Page 'About us'</returns>
        public IActionResult Tutorial()
        {
            ViewData["Title"] = "About us";
            return View();
        }
        /// <summary>
        /// Admin page for checking unconfirmed withdraws
        /// </summary>
        /// <param name="wallet">wallet of the user</param>
        /// <returns>Page with all unconfirmed withdraws</returns>
        [Authorize]
        public async Task<IActionResult> AdminWithdrawConfirm(string wallet)
        {
            if ((await methods.GetUserAsync()).UserName != adminName)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Withdraws = await _context.GetWithdrawsByWalletAndIsConfirmedAsync(wallet);
            return View();
        }
        /// <summary>
        /// Changes wallet of the user
        /// </summary>
        /// <param name="wallet">wallet of the user</param>
        /// <returns>Updated wallet address</returns>
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
            var user = await _context.GetUserByNameAsync(userName);
            user.WalletAddress = wallet;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Confirms withdraw only if Admin
        /// </summary>
        /// <param name="wallet">wallet of the user</param>
        /// <returns>Clears unchecked withdraws</returns>
        [Authorize]
        public async Task<IActionResult> AdminWithdrawConfirmed(string wallet)
        {
            if ((await methods.GetUserAsync()).UserName != adminName)
            {
                return RedirectToAction("Index");
            }
            var withdraws = await _context.GetWithdrawsByWalletAndIsConfirmedAsync(wallet);
            for (int i = 0; i < withdraws.Count(); i++)
            {
                withdraws[i].IsConfirmed = true;

                _context.Update(withdraws[i]);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminMenu", "Home");
        }
        /// <summary>
        /// Admin page - edits a user's account, checks for withdraws and deposits
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <param name="balance">balance of the user</param>
        /// <returns>Updated user's deposits and balance</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string id, double balance)
        {
            if ((await methods.GetUserAsync()).UserName != adminName)
            {
                return RedirectToAction("Index");
            }
            var user = await _context.GetUserByIdAsync(id);
            user.Balance += balance;
            user.MoneySpent += balance;
            user.PlayMoney = balance;
            var deposits = await _context.GetDepositsByUserIdAndIsConfirmedAsync(user, false);
            var user1 = await _context.GetUserByInviteLinkAsync(user);
            var task = await _context.GetTaskByUserAsync(user1);
            var task1 = await _context.GetTaskByUserAsync(user);
            StaticWorkMethods.IncreaseTaskAndBalance(balance, ref task1, ref user);
            StaticWorkMethods.ChangeLevelByMoneySpent(ref user);
            task.Count++;
            _context.Update(task);
            _context.Update(task1);
            for (int i = 0; i < deposits.Count(); i++)
            {
                var deposit = deposits[i];
                deposit.IsConfirmed = true;
                _context.Update(deposit);
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Validates withdraw request
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <param name="money">money to withdraw</param>
        /// <returns>ConfirmedWithdraw method if successful / Withdraw section if not / Index if no money to withdraw</returns>
        [Authorize]
        public async Task<IActionResult> TryWithdraw(string id, double money)
        {
            var user = await _context.GetUserByIdAsync(id);
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
            var withdrawReq = new WithdrawDataModel(Guid.NewGuid().ToString(), user.WalletAddress, money, user);
            ViewBag.Mon = money;
            return View("ConfirmWithdraw", withdrawReq);
        }
        /// <summary>
        /// Makes a withdraw instance
        /// </summary>
        /// <param name="id">id of the withdraw instance</param>
        /// <param name="money">money for withdraw</param>
        /// <param name="wallet">wallet for which the user withdraws</param>
        /// <param name="iduser">id of the user</param>
        /// <returns>Withdraw instance</returns>
        [Authorize]
        public async Task<IActionResult> ConfirmedWithdraw(string id, double money, string wallet, string iduser)
        {
            var user = await _context.GetUserByIdAsync(iduser);
            var withdrawReq = new WithdrawDataModel(id, wallet, money - (money * 0.06), user, false, DateTime.Now);
            user.Balance -= money;
            user.PlayMoney -= money;
            await _context.WithdrawDatas.AddAsync(withdrawReq);
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Goes to the withdraw section
        /// </summary>
        /// <returns>Withdraw page</returns>
        [Authorize]
        public async Task<IActionResult> Withdraw()
        {
            ViewData["Title"] = "Withdraw";
            ViewBag.ErrorBal = "";
            ViewBag.ErrorRef = "";
            ViewBag.ErrorNoMoney = "";
            var user = await methods.GetUserAsync();
            if (string.IsNullOrEmpty(user.WalletAddress))
            {
                return RedirectToAction("Profile", false);
            }
            return View(user);
        }
        /// <summary>
        /// Main page, shows bitcoin supply and the mining option
        /// </summary>
        /// <returns>Index page</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Mine";
            ViewBag.ErrorCh = "";
            var user = await _context.GetUserByNameAsync(userName);
            var task = await _context.GetTaskByUserAsync(user);
            ViewBag.User = user;
            string block = methods.RandomizeBlockchain();
            bitcoinSupply -= 0.000396f;
            int countUsers = await methods.CountUsersAsync() + rnd.Next(300, 1200);
            ViewBag.Count = countUsers;
            ViewBag.BlockChain = block;
            ViewBag.Bitc = bitcoinSupply;
            return View(task);
        }
        /// <summary>
        /// Makes a pseudo-mining operation creating balance
        /// </summary>
        /// <param name="date">Gets the current date</param>
        /// <returns>Adds balance to the user's account</returns>
        [Authorize]
        public async Task<IActionResult> Mine(DateTime date)
        {
            var u = await _context.GetUserByNameAsync(userName);
            var t = await _context.GetTaskByUserAsync(u);
            if (u.Balance == 0)
            {
                return RedirectToAction("Deposit");
            }
            t.DateStarted = date;
            t.Count--;
            t.NewAccount = false;
            u.Balance = StaticWorkMethods.AddBalanceByLevel(u.Balance, u.PlayMoney, u.Level);
            _context.Update(t);
            _context.Update(u);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Validates if deposit is over 25$
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <param name="money">how much money he wants to deposit</param>
        /// <returns>A new deposit data model or gets you back to the same page</returns>
        [Authorize]
        public async Task<IActionResult> TryDeposit(string id, double money)
        {
            var user = await _context.GetUserByIdAsync(id);
            if (money < 25)
            {
                ViewBag.ErrorSum = "The minimum amount for deposit is 25$";
                return View("Deposit", user);
            }
            else
            {
                var depReq = new DepositDataModel(Guid.NewGuid().ToString(), user, user.Email, money);
                ViewBag.Wallet = methods.WalletSelector();
                return View("TryDeposit", depReq);
            }
        }
        /// <summary>
        /// Makes a deposit instance
        /// </summary>
        /// <param name="id">Id of the Deposit</param>
        /// <param name="money">How much money he wants to deposit</param>
        /// <param name="userid">Id of the User</param>
        /// <returns>Adds a DepositDataModel to Database</returns>
        [Authorize]
        public async Task<IActionResult> TryTheDeposit(string id, double money, string userid)
        {
            var user = await _context.GetUserByIdAsync(userid);
            var deposit = new DepositDataModel(id, user, user.Email, money, false, DateTime.Now);
            await _context.DepositDatas.AddAsync(deposit);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Person tries to deposit \n\n\n\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
};