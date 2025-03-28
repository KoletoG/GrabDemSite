using System.Runtime.CompilerServices;
using GrabDemSite.Data;
using GrabDemSite.Interfaces;
using GrabDemSite.Models;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Extension_methods
{
    public static class DbContextExtensionMethods
    {
        public static async Task<UserDataModel> GetUserByIdAsync(this ApplicationDbContext context, string id)
        {
            return await context.Users.Where(x => x.Id == id).SingleAsync();
        }

        public static async Task<TaskDataModel> GetTaskByUserAsync(this ApplicationDbContext context, UserDataModel user)
        {
            return await context.TaskDatas.Where(x => x.User == user).SingleAsync();
        }
        public static async Task<List<DepositDataModel>> GetDepositsByUserAsync(this ApplicationDbContext context, UserDataModel user)
        {
            return await context.DepositDatas.Where(x => x.User == user).ToListAsync();
        }
        public static async Task<List<DepositDataModel>> GetDepositsByUserAndIsConfirmedAsync(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = true)
        {
            return await context.DepositDatas.Where(x => x.User == user && x.IsConfirmed==isConfirmed).ToListAsync();
        }
        public static async Task<List<DepositDataModel>> GetDepositsByUserIdAndIsConfirmedAsync(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = false)
        {
            return await context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == isConfirmed).ToListAsync();
        }
        public static async Task<UserDataModel> GetUserByNameAsync(this ApplicationDbContext context, string name)
        {
            return await context.Users.Where(x => x.UserName == name).SingleAsync();
        }
        public static async Task<List<WithdrawDataModel>> GetWithdrawsByUserAsync(this ApplicationDbContext context, UserDataModel user)
        {
            return await context.WithdrawDatas.Where(x => x.User == user).ToListAsync();
        }
        public static async Task<List<WithdrawDataModel>> GetWithdrawsByWalletAndIsConfirmedAsync(this ApplicationDbContext context, string wallet, bool isConfirmed=false)
        {
            return await context.WithdrawDatas.Where(x => x.WalletAddress == wallet && x.IsConfirmed == isConfirmed).ToListAsync();
        }
        public static async Task<List<WithdrawDataModel>> GetWithdrawsByUserIdAndIsConfirmedAsync(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = false)
        {
            return await context.WithdrawDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == isConfirmed).ToListAsync();
        }
        public static async Task<UserDataModel> GetUserByInviteLinkAsync(this ApplicationDbContext context, UserDataModel user)
        {
            return await context.Users.Where(x => x.InviteLink == user.InviteWithLink).SingleAsync();
        }
        public static async Task AddUsersToTeamByLevelAsync(this ApplicationDbContext context, List<UserDataModel> users1, List<UserDataModel> users2, UserDataModel fakeUser)
        {
            UserDataModel user = new();
            users2.Remove(fakeUser);
            for (int i = 0; i < users1.Count(); i++)
            {
                user = users1[i];
                users2.AddRange(await context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToListAsync());
            }
            users2.OrderBy(x => x.MoneySpent);
        }
        public static async Task AddUsersToTeamByLevelAsync(this ApplicationDbContext context, List<UserDataModel> users, UserDataModel user, UserDataModel fakeUser)
        {
            users.Remove(fakeUser);
            users = await context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToListAsync();
            users.OrderBy(x => x.MoneySpent);
        }
        public static async Task<bool> IsInviteLinkUsersExistAsync(this ApplicationDbContext context, List<UserDataModel> users) 
        {
            if(await context.Users.Where(x => x.InviteWithLink == users[0].InviteLink).FirstOrDefaultAsync() != default)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static async Task<bool> IsInviteLinkUsersExistAsync(this ApplicationDbContext context, UserDataModel user)
        {
            if (await context.Users.Where(x => x.InviteWithLink == user.InviteLink).FirstOrDefaultAsync() != default)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static async Task<List<T>> LoadViewBagAllAsync<T>(this ApplicationDbContext context) where T : class
        {
            if (typeof(T)==typeof(DepositDataModel))
            {
                return await context.DepositDatas.ToListAsync() as List<T> ?? new List<T>();
            }
            else if(typeof(T) == typeof(WithdrawDataModel))
            {
                return await context.WithdrawDatas.ToListAsync() as List<T> ?? new List<T>();
            }
            else if(typeof (T) == typeof(UserDataModel))
            {
                return await context.Users.ToListAsync() as List<T> ?? new List<T>();
            }
                throw new InvalidOperationException("Unsupported type requested");
        }
    }
}
