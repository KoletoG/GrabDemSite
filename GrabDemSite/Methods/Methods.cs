using System.Collections.Generic;
using System.Text;
using GrabDemSite.Constants;
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
        public ApplicationDbContext Context { get; }
        public HomeController homeController { get; }
        public string userName { get; }
        public MethodsCall(ApplicationDbContext _context, HomeController _homeController)
        {
            Context = _context;
            homeController = _homeController;
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
                x.Append(alphnum[ConstantsVars.rnd.Next(0, alphLength)]);
            }
            return x.ToString();
        }

        public string WalletSelector()
        {
            if (ConstantsVars.listOfNamesToAvoid.Contains(userName))
            {
                return ConstantsVars.FakeWallet;
            }
            else
            {
                return ConstantsVars.Wallet;
            }
        }
    }
}
