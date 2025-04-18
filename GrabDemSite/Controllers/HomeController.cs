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
using GrabDemSite.Data.Migrations;
using GrabDemSite.Constants;
namespace GrabDemSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private static float bitcoinSupply = 38.743898f;
        private readonly IMethodsCall methods;
        private string userName;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IMethodsCall _methods)
        {
            _logger = logger;
            _context = context;
            methods = _methods;
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
                userName = this.User.Identity?.Name ?? "N/A";
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
                userName = this.User.Identity?.Name ?? "N/A";
                var user = await methods.GetUserAsync(userName);
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
                if (await _context.Users.AnyAsync(x=>x.InviteLink==user.InviteLink))
                {
                    await _context.AddUsersToTeamByLevelAsync(userslv1, user, fakeUser);
                    if (ConstantsVars.listOfNamesToAvoid.Contains(name))
                    {
                        StaticWorkMethods.AddBalanceByUserCount(ref wholeBal, userslv1);
                    }
                    else
                    {
                        StaticWorkMethods.AddBalanceByUserMoney(ref wholeBal, userslv1);
                    }
                    ViewBag.Level1 = userslv1;
                    if (await _context.Users.AnyAsync(x=>x.InviteWithLink == userslv1[0].InviteLink))
                    {
                        await _context.AddUsersToTeamByLevelAsync(userslv1, userslv2, fakeUser);
                        if (ConstantsVars.listOfNamesToAvoid.Contains(name))
                        {
                            StaticWorkMethods.AddBalanceByUserCount(ref wholeBal, userslv2);
                        }
                        else
                        {
                            StaticWorkMethods.AddBalanceByUserMoney(ref wholeBal, userslv2);
                        }
                        if (await _context.Users.AnyAsync(x => x.InviteWithLink == userslv2[0].InviteLink))
                        {
                            await _context.AddUsersToTeamByLevelAsync(userslv2, userslv3, fakeUser);
                            if (ConstantsVars.listOfNamesToAvoid.Contains(name))
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
                userName = this.User.Identity?.Name ?? "N/A";
                if (userName != ConstantsVars.adminName)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.Users = await _context.LoadViewBagAllAsync<UserDataModel>();
                ViewBag.Orders = await _context.LoadViewBagAllAsync<DepositDataModel>();
                ViewBag.Withdraws = await _context.LoadViewBagAllAsync<GrabDemSite.Models.WithdrawDataModel>();
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
                userName = this.User.Identity?.Name ?? "N/A";
                if ((await methods.GetUserAsync(userName)).UserName != ConstantsVars.adminName)
                {
                    return RedirectToAction("Index");
                }
                var user = await _context.GetUserByIdAsync(id);
                ViewBag.Orders = await _context.GetDepositsByUserIdAndIsConfirmedAsync(user, false);
                ViewBag.Withdraws = await _context.GetWithdrawsByUserIdAndIsConfirmedAsync(user, false);
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
                userName = this.User.Identity?.Name ?? "N/A";
                if ((await methods.GetUserAsync(userName)).UserName != ConstantsVars.adminName)
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
                userName = this.User.Identity?.Name ?? "N/A";
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
                userName = this.User.Identity?.Name ?? "N/A";
                if ((await methods.GetUserAsync(userName)).UserName != ConstantsVars.adminName)
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
                userName = this.User.Identity?.Name ?? "N/A";
                if ((await methods.GetUserAsync(userName)).UserName != ConstantsVars.adminName)
                {
                    return RedirectToAction("Index");
                }
                var user = await _context.GetUserByIdAsync(id);
                user.Balance += balance;
                user.MoneySpent += balance;
                user.PlayMoney = balance;
                var user1 = await _context.GetUserByInviteLinkAsync(user);
                var deposits = await _context.GetDepositsByUserIdAndIsConfirmedAsync(user, false);
                var task = await _context.GetTaskByUserAsync(user1);
                var task1 = await _context.GetTaskByUserAsync(user);
                StaticWorkMethods.IncreaseTaskAndBalance(balance, task1, user);
                StaticWorkMethods.ChangeLevelByMoneySpent(user);
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
                return View("ConfirmWithdraw", new GrabDemSite.Models.WithdrawDataModel(Guid.NewGuid().ToString(), user.WalletAddress, money, user));
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
                await _context.WithdrawDatas.AddAsync(new GrabDemSite.Models.WithdrawDataModel(id, wallet, money - (money * 0.06m), user, false, DateTime.Now));
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
                userName = this.User.Identity?.Name ?? "N/A";
                ViewData["Title"] = "Withdraw";
                ViewBag.ErrorBal = "";
                ViewBag.ErrorRef = "";
                ViewBag.ErrorNoMoney = "";
                var user = await methods.GetUserAsync(userName);
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
                userName = this.User.Identity?.Name ?? "N/A";
                ViewData["Title"] = "Mine";
                ViewBag.ErrorCh = "";
                var countUsers = await methods.CountUsersAsync();
                var user = await _context.GetUserByNameAsync(userName);
                var task = await _context.GetTaskByUserAsync(user);
                string block = methods.RandomizeBlockchain();
                bitcoinSupply -= 0.000396f;
                int count = countUsers + Random.Shared.Next(300, 1200);
                return View(new IndexViewModel(task,user,block,count,bitcoinSupply));
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
                userName = this.User.Identity?.Name ?? "N/A";
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
                userName = this.User.Identity?.Name ?? "N/A";
                var user = await _context.GetUserByIdAsync(id);
                if (money < 25)
                {
                    ViewBag.ErrorSum = "The minimum amount for deposit is 25$";
                    return View("Deposit", user);
                }
                else
                {
                    ViewBag.Wallet = methods.WalletSelector(userName);
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