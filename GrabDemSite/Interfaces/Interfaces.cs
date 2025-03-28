﻿using GrabDemSite.Controllers;
using GrabDemSite.Data;
using GrabDemSite.Models;

namespace GrabDemSite.Interfaces
{
    public interface ITransactionDataModel
    {
        string Id { get; set; }
        UserDataModel User { get; set; }
        bool IsConfirmed { get; set; }
        DateTime DateCreated { get; set; }
        double Money { get; set; }
    }
    public interface IUserDataModel
    {
        string Id { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string InviteLink { get; set; }
        string InviteWithLink { get; set; }
        DateTimeOffset DateCreated { get; set; }
        double Balance { get; set; }
        double MoneySpent { get; set; }
        string WalletAddress { get; set; }
        short InviteCount { get; set; }
        double PlayMoney { get; set; }
        byte Level { get; set; }
    }
    public interface IMethodsCall
    {
        const string alphnum="1234567890abcdefghijklmnopqrstuvwxyz";
        Random Rnd {  get; }
        public ApplicationDbContext Context { get; }
        public HomeController homeController { get; }
        string[] ListOfNamesToAvoid { get; }
        string userName { get;}
        Task<int> CountUsersAsync();
        Task<UserDataModel> GetUserAsync();
        string RandomizeBlockchain();
        string WalletSelector();
    }
}
