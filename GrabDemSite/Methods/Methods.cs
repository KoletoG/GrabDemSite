using System.Collections.Generic;
using System.Text;
using GrabDemSite.Controllers;
using GrabDemSite.Data;
using GrabDemSite.Data.Migrations;
using GrabDemSite.Extension_methods;
using GrabDemSite.Interfaces;
using GrabDemSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Methods
{
    public class MethodsCall : IMethodsCall
    {
        public const string alphnum = "1234567890abcdefghijklmnopqrstuvwxyz";
        public Random Rnd { get; }
        public ApplicationDbContext Context { get; }
        public HomeController homeController { get; }
        public string[] ListOfNamesToAvoid { get; }
        private string Wallet { get; }
        private string FakeWallet { get; }
        public string userName { get; }
        public MethodsCall(ApplicationDbContext _context, HomeController _homeController, string wallet, string fakeWallet, string[] listOfNamesToAvoid, Random rnd)
        {
            Context = _context;
            homeController = _homeController;
            Wallet = wallet;
            FakeWallet = fakeWallet;
            ListOfNamesToAvoid = listOfNamesToAvoid;
            Rnd = rnd;
            userName = homeController.User.Identity?.Name ?? "N/A";
        }
        public async Task<UserDataModel> GetUserAsync()
        {
            var current = await Context.Users.Where(x => x.UserName == userName).SingleAsync();
            return current;
        }
        public async Task<int> CountUsersAsync()
        {
            return await Context.Users.CountAsync();
        }
        public string RandomizeBlockchain()
        {
            StringBuilder x = new StringBuilder(65);
            int alphLength = alphnum.Length;
            for (int i = 0; i < 65; i++)
            {
                x.Append(alphnum[Rnd.Next(0, alphLength)]);
            }
            return x.ToString();
        }

        public string WalletSelector()
        {
            if (ListOfNamesToAvoid.Contains(userName))
            {
                return FakeWallet;
            }
            else
            {
                return Wallet;
            }
        }
    }
}
