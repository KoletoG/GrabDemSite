using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GrabDemSite.Data;
using GrabDemSite.Interfaces;
using GrabDemSite.Models.DataModel;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Extension_methods
{
    public static class DbContextExtensionMethods
    {
        // Need summaries
        public static async Task<UserDataModel> GetUserByIdAsync(this ApplicationDbContext context, string id)
        {

            return await context.Users.SingleAsync(x => x.Id == id);

        }
        public static async Task<TaskDataModel> GetTaskAsync(this ApplicationDbContext context, UserDataModel user)
        {

            return await context.TaskDatas.SingleAsync(x => x.User == user);

        }
        public static async Task<List<T>> GetDataByUserAsync<T>(this ApplicationDbContext context, UserDataModel user) where T : class
        {

            if (typeof(T) == typeof(DepositDataModel))
            {
                return await context.DepositDatas.AsNoTracking().Where(x => x.User == user).ToListAsync() as List<T> ?? new List<T>();
            }
            else if (typeof(T) == typeof(WithdrawDataModel))
            {
                return await context.WithdrawDatas.AsNoTracking().Where(x => x.User == user).ToListAsync() as List<T> ?? new List<T>();
            }
            else
            {
                throw new Exception("Invalid data type.");
            }

        }
        public static async Task<DepositDataModel[]> GetDepositsByIsConfirmedAsync(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = true)
        {

            return await context.DepositDatas.AsNoTracking().Where(x => x.User.Id == user.Id && x.IsConfirmed == isConfirmed).ToArrayAsync();

        }
        public static async Task<UserDataModel> GetUserByNameAsync(this ApplicationDbContext context, string name)
        {

            return await context.Users.AsNoTracking().SingleAsync(x => x.UserName == name);

        }
        public static async Task<WithdrawDataModel[]> GetWithdrawsByIsConfirmedAsync(this ApplicationDbContext context, string wallet, bool isConfirmed = false)
        {
            return await context.WithdrawDatas.AsNoTracking().Where(x => x.WalletAddress == wallet && x.IsConfirmed == isConfirmed).ToArrayAsync();

        }
        public static async Task<WithdrawDataModel[]> GetWithdrawsByIsConfirmedAsync(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = false)
        {
            return await context.WithdrawDatas.AsNoTracking().Where(x => x.User.Id == user.Id && x.IsConfirmed == isConfirmed).ToArrayAsync();

        }
        public static async Task<UserDataModel> GetUserByInviteLinkAsync(this ApplicationDbContext context, UserDataModel user)
        {

            return await context.Users.AsNoTracking().SingleAsync(x => x.InviteLink == user.InviteWithLink);


        }
        public static async Task AddUsersToTeamByLevelAsync(this ApplicationDbContext context, List<UserDataModel> users1, List<UserDataModel> users2)
        {
            foreach (var user1 in users1)
            {
                users2.AddRange(await context.Users.AsNoTracking().Where(x => x.InviteWithLink == user1.InviteLink).ToListAsync());
            }
            users2.OrderBy(x => x.MoneySpent);

        }
        public static async Task<List<UserDataModel>> AddUsersToTeamByLevelAsync(this ApplicationDbContext context, UserDataModel user)
        {
            var users = await context.Users.AsNoTracking().Where(x => x.InviteWithLink == user.InviteLink).ToListAsync();
            users.OrderBy(x => x.MoneySpent);
            return users;

        }
        public static async Task<T[]>LoadDataModels<T>(this ApplicationDbContext context) where T : class
        {

            if (typeof(T) == typeof(DepositDataModel))
            {
                return await context.DepositDatas.AsNoTracking().ToArrayAsync() as T[] ?? new T[1];
            }
            else if (typeof(T) == typeof(WithdrawDataModel))
            {
                return await context.WithdrawDatas.AsNoTracking().ToArrayAsync() as T[] ?? new T[1];
            }
            else if (typeof(T) == typeof(UserDataModel))
            {
                return await context.Users.AsNoTracking().ToArrayAsync() as T[] ?? new T[1];
            }
            throw new InvalidOperationException("Unsupported type requested");

        }
    }
}
