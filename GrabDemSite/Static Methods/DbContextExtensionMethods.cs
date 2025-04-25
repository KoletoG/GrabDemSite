using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Transactions;
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

            return await context.Users.FirstAsync(x => x.Id == id);

        }
        public static async Task<TaskDataModel> GetTaskAsync(this ApplicationDbContext context, UserDataModel user)
        {

            return await context.TaskDatas.FirstAsync(x => x.User == user);

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

            return await context.Users.AsNoTracking().FirstAsync(x => x.UserName == name);

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

            return await context.Users.AsNoTracking().FirstAsync(x => x.InviteLink == user.InviteWithLink);


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
        public static async Task<T[]> LoadDataModels<T>(this ApplicationDbContext context) where T : class
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

        public static async Task<decimal> LoadLevelsLists(this ApplicationDbContext _context, List<UserDataModel> lv1, List<UserDataModel> lv2, List<UserDataModel> lv3, UserDataModel user, decimal balance, int count = 1, bool nameToAvoid = false)
        {
            if (await _context.Users.AsNoTracking().AnyAsync(x => x.InviteWithLink == user.InviteLink))
            {
                var users = await _context.Users.AsNoTracking().Where(x => x.InviteWithLink == user.InviteLink).ToListAsync();
                if (!nameToAvoid)
                {
                    balance += users.Sum(x => x.MoneySpent);
                }
                else
                {
                    balance += users.Count() * 25;
                }
                if (count == 1)
                {
                    lv1.AddRange(users);
                    foreach (var user1 in lv1)
                    {
                        balance += await LoadLevelsLists(_context, lv1, lv2, lv3, user1, balance, 2, nameToAvoid);
                    }
                }
                else if (count == 2)
                {
                    lv2.AddRange(users);
                    foreach (var user2 in lv2)
                    {
                        balance += await LoadLevelsLists(_context, lv1, lv2, lv3, user2, balance, 3, nameToAvoid);
                    }
                }
                else if (count == 3)
                {
                    lv3.AddRange(users);
                    return balance;
                }
            }
            return balance;
        }

        public static async Task<decimal> CalculateBalance(this ApplicationDbContext _context, UserDataModel user, decimal balance, int count = 1, bool nameToAvoid = false)
        {
            if (count > 3)
            {
                return balance;
            }
            balance += user.InvitedUsers.Sum(x => x.MoneySpent);
            count++;
            foreach (var user1 in user.InvitedUsers)
            {
                balance += await CalculateBalance(_context, user1, balance, count);
            }
            return balance;
        }
        public static void LoadUserLevels(this ApplicationDbContext _context, List<string> lv1,List<string> lv2, List<string> lv3,UserDataModel user,int count = 1)
        {
            if (count == 1)
            {
                lv1 = user.InvitedUsers.Select(x=>x.UserName).ToList();
                foreach(var user1 in lv1)
                {
                    LoadUserLevels(_context, lv1, lv2, lv3, user, 2);
                }
            }
            else if (count == 2)
            {
                lv2.AddRange(user.InvitedUsers.Select(x => x.UserName).ToList());
                foreach (var user1 in lv2)
                {
                    LoadUserLevels(_context, lv1, lv2, lv3, user, 3);
                }
            }
            else if (count == 3)
            {
                lv3.AddRange(user.InvitedUsers.Select(x => x.UserName).ToList());
            }
        }
    }
}
