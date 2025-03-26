﻿using GrabDemSite.Data;
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
            userName = this.User.Identity.Name;
        }
        [Authorize]
        public IActionResult DeleteAccount(string id)
        {
            var user = _context.GetUserById(id);
            var task = _context.GetTaskByUser(user);
            var deposits = _context.GetDepositsByUser(user);
            var withdraws = _context.GetWithdrawsByUser(user);
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
        public IActionResult Deposit()
        {
            ViewData["Title"] = "Deposit";
            ViewBag.ErrorSum = "";
            var user = _context.GetUserByName(userName);
            if (string.IsNullOrEmpty(user.WalletAddress))
            {
                return RedirectToAction("Profile", false);
            }
            return View(user);
        }
        [Authorize]
        public IActionResult Profile(bool tr)
        {
            var user = methods.GetUser();
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
            ViewBag.DepositOrders = _context.GetDepositsByUserAndIsConfirmed(user);
            ViewBag.WithdrawOrders = _context.GetWithdrawsByUser(user);
            if (tr == false)
            {
                ViewBag.Error = "You need to set your wallet first";
            }
            else
            {
                ViewBag.Error = null;
            }
            if (_context.IsInviteLinkUsersExist(user))
            {
                _context.AddUsersToTeamByLevel(ref userslv1, user, fakeUser);
                if (name.IsNameToAvoidHere(listOfNamesToAvoid))
                {
                    StaticWorkMethods.AddBalanceByUserCount(ref wholeBal, userslv1);
                }
                else
                {
                    StaticWorkMethods.AddBalanceByUserMoney(ref wholeBal, userslv1);
                }
                ViewBag.Level1 = userslv1;
                if (_context.IsInviteLinkUsersExist(userslv1))
                {
                    _context.AddUsersToTeamByLevel(ref userslv1, ref userslv2, fakeUser);
                    if (name.IsNameToAvoidHere(listOfNamesToAvoid))
                    {
                        StaticWorkMethods.AddBalanceByUserCount(ref wholeBal, userslv2);
                    }
                    else
                    {
                        StaticWorkMethods.AddBalanceByUserMoney(ref wholeBal, userslv2);
                    }
                    if (_context.IsInviteLinkUsersExist(userslv2))
                    {
                        _context.AddUsersToTeamByLevel(ref userslv2, ref userslv3, fakeUser);
                        if (name.IsNameToAvoidHere(listOfNamesToAvoid))
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
        [Authorize]
        public IActionResult AdminMenu()
        {
            if (userName != adminName)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Orders = _context.DepositDatas.ToList();
            ViewBag.Withdraws = _context.WithdrawDatas.ToList();
            return View();
        }
        [Authorize]
        public IActionResult Edit(string id)
        {
            if (methods.GetUser().UserName != adminName)
            {
                return RedirectToAction("Index");
            }
            var user = _context.GetUserById(id);
            ViewBag.Orders = _context.GetDepositsByUserIdAndIsConfirmed(user, false);
            ViewBag.Withdraws = _context.GetWithdrawsByUserIdAndIsConfirmed(user, false);

            return View(user);
        }
        public IActionResult Tutorial()
        {
            ViewData["Title"] = "About us";
            return View();
        }
        [Authorize]
        public IActionResult AdminWithdrawConfirm(string wallet)
        {
            if (methods.GetUser().UserName != "adminName")
            {
                return RedirectToAction("Index");
            }
            ViewBag.Withdraws = _context.GetWithdrawsByWalletAndIsConfirmed(wallet);
            return View();
        }
        [Authorize]
        public IActionResult ChangeWallet(string wallet)
        {
            if (wallet == null)
            {
                return RedirectToAction("Profile");
            }
            if (!wallet.StartsWith('1') && !wallet.StartsWith('3') && !wallet.StartsWith("bc1"))
            {
                return RedirectToAction("Profile");
            }
            var user = _context.GetUserByName(userName);
            user.WalletAddress = wallet;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult AdminWithdrawConfirmed(string wallet)
        {
            if (methods.GetUser().UserName != "adminName")
            {
                return RedirectToAction("Index");
            }
            List<WithdrawDataModel> withdraws = _context.GetWithdrawsByWalletAndIsConfirmed(wallet);

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
        public IActionResult Edit(string id, double balance)
        {
            if (methods.GetUser().UserName != "adminName")
            {
                return RedirectToAction("Index");
            }
            var user = _context.GetUserById(id);
            user.Balance += balance;
            user.MoneySpent += balance;
            user.PlayMoney = balance;
            List<DepositDataModel> deposits = _context.GetDepositsByUserIdAndIsConfirmed(user,false);
            UserDataModel user1 = _context.GetUserByInviteLink(user);
            var task = _context.GetTaskByUser(user1);
            var task1 = _context.GetTaskByUser(user);
            StaticWorkMethods.IncreaseTaskAndBalance(balance, ref task1, ref user);
            StaticWorkMethods.ChangeLevelByMoneySpent(ref user);
            task.Count++;
            _context.Update(task);
            _context.Update(task1);
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
            WithdrawDataModel withdrawReq = new WithdrawDataModel(Guid.NewGuid().ToString(), user.WalletAddress, money, user);
            ViewBag.Mon = money;
            return View("ConfirmWithdraw", withdrawReq);
        }

        [Authorize]
        public IActionResult ConfirmedWithdraw(string id, double money, string wallet, string iduser)
        {
            var user = _context.GetUserById(iduser);
            var withdrawReq = new WithdrawDataModel(id, wallet, money - (money * 0.06), user, false, DateTime.Now);
            user.Balance -= money;
            user.PlayMoney -= money;
            _context.WithdrawDatas.Add(withdrawReq);
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Withdraw()
        {
            ViewData["Title"] = "Withdraw";
            ViewBag.ErrorBal = "";
            ViewBag.ErrorRef = "";
            ViewBag.ErrorNoMoney = "";
            var user = methods.GetUser();
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
        [Authorize]
        public IActionResult Index()
        {
            ViewData["Title"] = "Mine";
            ViewBag.ErrorCh = "";
            var user = _context.GetUserByName(userName);
            var task = _context.GetTaskByUser(user);
            ViewBag.User = user;
            string block = methods.RandomizeBlockchain();
            bitcoinSupply -= 0.000396f;
            int countUsers = methods.CountUsers() + rnd.Next(300, 1200);
            ViewBag.Count = countUsers;
            ViewBag.BlockChain = block;
            ViewBag.Bitc = bitcoinSupply;
            return View(task);
        }
        [Authorize]
        public IActionResult Mine(DateTime date)
        {
            UserDataModel u = _context.GetUserByName(userName);
            var t = _context.GetTaskByUser(u);
            if (u.Balance == 0)
            {
                return RedirectToAction("Deposit");
            }
            t.DateStarted = date;
            t.Count--;
            t.NewAccount = false;
            _context.Update(t);
            u.Balance = StaticWorkMethods.AddBalanceByLevel(u.Balance, u.PlayMoney, u.Level);
            _context.Update(u);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult CompletedTask()
        {
            var user = _context.GetUserByName(userName);
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult TryDeposit(string id, double money)
        {
            var user = _context.GetUserById(id);
            if (money < 25)
            {
                ViewBag.ErrorSum = "The minimum amount for deposit is 25$";
                return View("Deposit", user);
            }
            else
            {
                DepositDataModel depReq = new DepositDataModel(Guid.NewGuid().ToString(), user, user.Email, money);
                ViewBag.Wallet = methods.WalletSelector();
                return View("TryDeposit", depReq);
            }
        }
        [Authorize]
        public IActionResult TryTheDeposit(string id, double money, string userid)
        {
            var user = _context.GetUserById(id);
            DepositDataModel deposit = new DepositDataModel(id, user, user.Email, money, false, DateTime.Now);
            _context.DepositDatas.Add(deposit);
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