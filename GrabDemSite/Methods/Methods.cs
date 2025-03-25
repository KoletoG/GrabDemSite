using GrabDemSite.Controllers;
using GrabDemSite.Data;
using GrabDemSite.Data.Migrations;
using GrabDemSite.Extension_methods;
using GrabDemSite.Models;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Methods
{
    public class MethodsCall
    {
        private const string alphnum = "1234567890abcdefghijklmnopqrstuvwxyz"; 
        private Random Rnd { get; }
        private ApplicationDbContext Context { get; set; }
        private HomeController homeController { get; set; }
        private string[] ListOfNamesToAvoid { get; }
        private string Wallet {get;}
        private string FakeWallet {get;}
        public MethodsCall(ApplicationDbContext _context,HomeController _homeController,string wallet, string fakeWallet, string[] listOfNamesToAvoid,Random rnd)
        {
            Context = _context;
            homeController = _homeController;
            Wallet = wallet;
            FakeWallet = fakeWallet;
            ListOfNamesToAvoid = listOfNamesToAvoid;
            Rnd = rnd;
        }

        public UserDataModel GetUser()
        {
            var current = Context.Users.Where(x => x.UserName == homeController.User.Identity.Name).Single();
            return current;
        }
        public int CountUsers()
        {
            List<UserDataModel> users = Context.Users.ToList();
            return users.Count();
        }
        public string RandomizeBlockchain()
        {
            string x = "";
            for (int i = 1; i <= 65; i++)
            {
                x += alphnum[Rnd.Next(0, alphnum.Length)];
            }
            return x;
        }
        public string WalletSelector()
        {
            string name = homeController.User.Identity.Name;
            if (name.IsNameToAvoidHere(ListOfNamesToAvoid))
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
