using GrabDemSite.Controllers;
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
        decimal Money { get; set; }
    }
    public interface IUserDataModel
    {
        string Id { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string InviteLink { get; set; }
        string InviteWithLink { get; set; }
        DateTimeOffset DateCreated { get; set; }
        decimal Balance { get; set; }
        decimal MoneySpent { get; set; }
        string WalletAddress { get; set; }
        short InviteCount { get; set; }
        decimal PlayMoney { get; set; }
        byte Level { get; set; }
    }
    public interface IMethodsCall
    {
        public ApplicationDbContext Context { get; }
        Task<int> CountUsersAsync();
        Task<UserDataModel> GetUserAsync(string userName);
        string RandomizeBlockchain();
        string WalletSelector(string userName);
    }
}
