using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GrabDemSite.Data;
using GrabDemSite.Interfaces;
using GrabDemSite.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Extension_methods
{
    public static class DbContextExtensionMethods
    {
        public static async Task<UserDataModel> GetUserByIdAsync(this ApplicationDbContext context, string id)
        {
            try
            {
                return await context.Users.SingleAsync(x => x.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<TaskDataModel> GetTaskByUserAsync(this ApplicationDbContext context, UserDataModel user)
        {
            try
            {
                return await context.TaskDatas.SingleAsync(x => x.User == user);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<List<DepositDataModel>> GetDepositsByUserAsync(this ApplicationDbContext context, UserDataModel user)
        {
            try
            {
                return await context.DepositDatas.Where(x => x.User == user).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<List<DepositDataModel>> GetDepositsByUserAndIsConfirmedAsync(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = true)
        {
            try
            {
                return await context.DepositDatas.Where(x => x.User == user && x.IsConfirmed == isConfirmed).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<List<DepositDataModel>> GetDepositsByUserIdAndIsConfirmedAsync(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = false)
        {
            try
            {
                return await context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == isConfirmed).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<UserDataModel> GetUserByNameAsync(this ApplicationDbContext context, string name)
        {
            try
            {
                return await context.Users.SingleAsync(x => x.UserName == name);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<List<WithdrawDataModel>> GetWithdrawsByUserAsync(this ApplicationDbContext context, UserDataModel user)
        {
            try
            {
                return await context.WithdrawDatas.Where(x => x.User == user).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<List<WithdrawDataModel>> GetWithdrawsByWalletAndIsConfirmedAsync(this ApplicationDbContext context, string wallet, bool isConfirmed = false)
        {
            try
            {
                return await context.WithdrawDatas.Where(x => x.WalletAddress == wallet && x.IsConfirmed == isConfirmed).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<List<WithdrawDataModel>> GetWithdrawsByUserIdAndIsConfirmedAsync(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = false)
        {
            try
            {
                return await context.WithdrawDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == isConfirmed).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<UserDataModel> GetUserByInviteLinkAsync(this ApplicationDbContext context, UserDataModel user)
        {
            try
            {
                return await context.Users.SingleAsync(x => x.InviteLink == user.InviteWithLink);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task AddUsersToTeamByLevelAsync(this ApplicationDbContext context, List<UserDataModel> users1, List<UserDataModel> users2)
        {
            try
            {
                foreach (var user1 in users1)
                {
                    users2.AddRange(await context.Users.Where(x => x.InviteWithLink == user1.InviteLink).ToListAsync());
                }
                users2.OrderBy(x => x.MoneySpent);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<List<UserDataModel>> AddUsersToTeamByLevelAsync(this ApplicationDbContext context, UserDataModel user)
        {
            try
            {
                var users = new List<UserDataModel>();
                users = await context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToListAsync();
                users.OrderBy(x => x.MoneySpent);
                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task<List<T>> LoadViewBagAllAsync<T>(this ApplicationDbContext context) where T : class
        {
            try
            {
                if (typeof(T) == typeof(DepositDataModel))
                {
                    return await context.DepositDatas.ToListAsync() as List<T> ?? new List<T>();
                }
                else if (typeof(T) == typeof(WithdrawDataModel))
                {
                    return await context.WithdrawDatas.ToListAsync() as List<T> ?? new List<T>();
                }
                else if (typeof(T) == typeof(UserDataModel))
                {
                    return await context.Users.ToListAsync() as List<T> ?? new List<T>();
                }
                throw new InvalidOperationException("Unsupported type requested");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
