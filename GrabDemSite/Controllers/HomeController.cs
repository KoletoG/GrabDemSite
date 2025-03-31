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
using GrabDemSite.Interfaces;
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
        private readonly IMethodsCall methods;
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
            try
            {
                var user = await _context.GetUserByIdAsync(id);
                var taskTask = _context.GetTaskByUserAsync(user);
                var depositsTask = _context.GetDepositsByUserAsync(user);
                var withdrawsTask = _context.GetWithdrawsByUserAsync(user);
                await Task.WhenAll(taskTask, depositsTask, withdrawsTask);
                var deposits = await depositsTask;
                var withdraws = await withdrawsTask;
                var task = await taskTask;
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Contact page
        /// </summary>
        /// <returns>Contact page</returns>
        [Authorize]
        public IActionResult Contact()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Redirects user to deposit page
        /// </summary>
        /// <returns>Deposit page</returns>
        [Authorize]
        public async Task<IActionResult> Deposit()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Shows your profile information
        /// </summary>
        /// <param name="tr">checks if wallet is set</param>
        /// <returns>Profile page</returns>
        [Authorize]
        public async Task<IActionResult> Profile(bool tr)
        {
            try
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
                decimal wholeBal = 0m;
                string name = user.UserName;
                var depositsTask = _context.GetDepositsByUserAndIsConfirmedAsync(user);
                var withdrawsTask = _context.GetWithdrawsByUserAsync(user);
                await Task.WhenAll(depositsTask, withdrawsTask);
                ViewBag.DepositOrders = await depositsTask;
                ViewBag.WithdrawOrders = await withdrawsTask;
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
                    if (await _context.IsInviteLinkUsersExistAsync(userslv1.FirstOrDefault()))
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
                        if (await _context.IsInviteLinkUsersExistAsync(userslv2.FirstOrDefault()))
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Shows every user and their withdraws and deposits
        /// </summary>
        /// <returns>Page with all important information about users</returns>
        [Authorize]
        public async Task<IActionResult> AdminMenu()
        {
            try
            {
                if (userName != adminName)
                {
                    return RedirectToAction("Index");
                }
                var usersTask = _context.LoadViewBagAllAsync<UserDataModel>();
                var depositOrdersTask = _context.LoadViewBagAllAsync<DepositDataModel>();
                var withdrawOrdersTask = _context.LoadViewBagAllAsync<WithdrawDataModel>();
                await Task.WhenAll(usersTask, depositOrdersTask, withdrawOrdersTask);
                ViewBag.Users = await usersTask;
                ViewBag.Orders = await depositOrdersTask;
                ViewBag.Orders = await withdrawOrdersTask;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Views unconfirmed deposits and withdraws / Admin only
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <returns>Page with unconfirmed deposits and withdraws</returns>
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if ((await methods.GetUserAsync()).UserName != adminName)
                {
                    return RedirectToAction("Index");
                }
                var user = await _context.GetUserByIdAsync(id);
                var order = _context.GetDepositsByUserIdAndIsConfirmedAsync(user, false);
                var withdraws = _context.GetWithdrawsByUserIdAndIsConfirmedAsync(user, false);
                await Task.WhenAll(withdraws, order);
                ViewBag.Orders = await order;
                ViewBag.Withdraws = await withdraws;
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// About us section
        /// </summary>
        /// <returns>Page 'About us'</returns>
        public IActionResult Tutorial()
        {
            try
            {
                ViewData["Title"] = "About us";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Admin page for checking unconfirmed withdraws
        /// </summary>
        /// <param name="wallet">wallet of the user</param>
        /// <returns>Page with all unconfirmed withdraws</returns>
        [Authorize]
        public async Task<IActionResult> AdminWithdrawConfirm(string wallet)
        {
            try
            {
                if ((await methods.GetUserAsync()).UserName != adminName)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.Withdraws = await _context.GetWithdrawsByWalletAndIsConfirmedAsync(wallet);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Changes wallet of the user
        /// </summary>
        /// <param name="wallet">wallet of the user</param>
        /// <returns>Updated wallet address</returns>
        [Authorize]
        public async Task<IActionResult> ChangeWallet(string wallet)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Confirms withdraw only if Admin
        /// </summary>
        /// <param name="wallet">wallet of the user</param>
        /// <returns>Clears unchecked withdraws</returns>
        [Authorize]
        public async Task<IActionResult> AdminWithdrawConfirmed(string wallet)
        {
            try
            {
                if ((await methods.GetUserAsync()).UserName != adminName)
                {
                    return RedirectToAction("Index");
                }
                var withdraws = await _context.GetWithdrawsByWalletAndIsConfirmedAsync(wallet);
                foreach (var withdraw in withdraws)
                {
                    withdraw.IsConfirmed = true;
                    _context.Update(withdraw);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminMenu", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Admin page - edits a user's account, checks for withdraws and deposits
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <param name="balance">balance of the user</param>
        /// <returns>Updated user's deposits and balance</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string id, decimal balance)
        {
            try
            {
                if ((await methods.GetUserAsync()).UserName != adminName)
                {
                    return RedirectToAction("Index");
                }
                var user = await _context.GetUserByIdAsync(id);
                user.Balance += balance;
                user.MoneySpent += balance;
                user.PlayMoney = balance;
                var user1 = await _context.GetUserByInviteLinkAsync(user);
                var depositsTask = _context.GetDepositsByUserIdAndIsConfirmedAsync(user, false);
                var taskTask = _context.GetTaskByUserAsync(user1);
                var task1Task = _context.GetTaskByUserAsync(user);
                await Task.WhenAll(depositsTask, taskTask, task1Task);
                var deposits = await depositsTask;
                var task1 = await task1Task;
                var task = await taskTask;
                StaticWorkMethods.IncreaseTaskAndBalance(balance, ref task1, ref user);
                StaticWorkMethods.ChangeLevelByMoneySpent(ref user);
                task.Count++;
                _context.Update(task);
                _context.Update(task1);
                foreach (var deposit in deposits)
                {
                    deposit.IsConfirmed = true;
                    _context.Update(deposit);
                }
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Validates withdraw request
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <param name="money">money to withdraw</param>
        /// <returns>ConfirmedWithdraw method if successful / Withdraw section if not / Index if no money to withdraw</returns>
        [Authorize]
        public async Task<IActionResult> TryWithdraw(string id, decimal money)
        {
            try
            {
                var user = await _context.GetUserByIdAsync(id);
                money -= money * 0.06m;
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
                ViewBag.Mon = money;
                return View("ConfirmWithdraw", new WithdrawDataModel(Guid.NewGuid().ToString(), user.WalletAddress, money, user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
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
        public async Task<IActionResult> ConfirmedWithdraw(string id, decimal money, string wallet, string iduser)
        {
            try
            {
                var user = await _context.GetUserByIdAsync(iduser);
                user.Balance -= money;
                user.PlayMoney -= money;
                await _context.WithdrawDatas.AddAsync(new WithdrawDataModel(id, wallet, money - (money * 0.06m), user, false, DateTime.Now));
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Goes to the withdraw section
        /// </summary>
        /// <returns>Withdraw page</returns>
        [Authorize]
        public async Task<IActionResult> Withdraw()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Main page, shows bitcoin supply and the mining option
        /// </summary>
        /// <returns>Index page</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Mine";
                ViewBag.ErrorCh = "";
                var countUsers = methods.CountUsersAsync();
                var user = _context.GetUserByNameAsync(userName);
                await Task.WhenAll(countUsers, user);
                var task = await _context.GetTaskByUserAsync(await user);
                ViewBag.User = user;
                string block = methods.RandomizeBlockchain();
                bitcoinSupply -= 0.000396f;
                ViewBag.Count = countUsers.Result + rnd.Next(300, 1200);
                ViewBag.BlockChain = block;
                ViewBag.Bitc = bitcoinSupply;
                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Makes a pseudo-mining operation creating balance
        /// </summary>
        /// <param name="date">Gets the current date</param>
        /// <returns>Adds balance to the user's account</returns>
        [Authorize]
        public async Task<IActionResult> Mine(DateTime date)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        /// Validates if deposit is over 25$
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <param name="money">how much money he wants to deposit</param>
        /// <returns>A new deposit data model or gets you back to the same page</returns>
        [Authorize]
        public async Task<IActionResult> TryDeposit(string id, decimal money)
        {
            try
            {
                var user = await _context.GetUserByIdAsync(id);
                if (money < 25)
                {
                    ViewBag.ErrorSum = "The minimum amount for deposit is 25$";
                    return View("Deposit", user);
                }
                else
                {
                    ViewBag.Wallet = methods.WalletSelector();
                    return View("TryDeposit", new DepositDataModel(Guid.NewGuid().ToString(), user, user.Email, money));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
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
        public async Task<IActionResult> TryTheDeposit(string id, decimal money, string userid)
        {
            try
            {
                var user = await _context.GetUserByIdAsync(userid);
                await _context.DepositDatas.AddAsync(new DepositDataModel(id, user, user.Email, money, false, DateTime.Now));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Person tries to deposit \n\n\n\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error");
            }
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
};