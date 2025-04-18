using System.Collections.Generic;
using System.Text;
using GrabDemSite.Constants;
using GrabDemSite.Controllers;
using GrabDemSite.Data;
using GrabDemSite.Data.Migrations;
using GrabDemSite.Extension_methods;
using GrabDemSite.Interfaces;
using GrabDemSite.Models.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Methods
{
    public class MethodsCall : IMethodsCall
    {
        public ApplicationDbContext Context { get; }
        public MethodsCall(ApplicationDbContext _context)
        {
            Context = _context;
        }
        public async Task<UserDataModel> GetUserAsync(string userName)
        {
            var current = await Context.Users.SingleAsync(x => x.UserName == userName);
            return current;
        }
        public async Task<int> CountUsersAsync()
        {
            return await Context.Users.CountAsync();
        }
        public string RandomizeBlockchain()
        {
            StringBuilder x = new StringBuilder(65);
            int alphLength = ConstantsVars.alphnum.Length;
            for (int i = 0; i < 65; i++)
            {
                x.Append(ConstantsVars.alphnum[Random.Shared.Next(0, alphLength)]);
            }
            return x.ToString();
        }

        public string WalletSelector(string userName)
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
