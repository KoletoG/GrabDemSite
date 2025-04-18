using GrabDemSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace GrabDemSite.Static_Methods
{
    public static class StaticWorkMethods
    {
        public static decimal AddBalanceByLevel(decimal balance, decimal playMoney, int level)
        {
            switch (level)
            {
                case 1: balance += playMoney * 0.05m; break;
                case 2: balance += playMoney * 0.06m; break;
                case 3: balance += playMoney * 0.07m; break;
            }
            return balance;
        }
        public static void IncreaseTaskAndBalance(decimal balance, TaskDataModel task, UserDataModel user)
        {

            if (balance >= 300)
            {
                task.Count += 6;
                user.Balance += 20;
            }
            else if (balance >= 200)
            {
                task.Count += 6;
                user.Balance += 15;
            }
            else if (balance >= 100)
            {
                task.Count += 5;
                user.Balance += 10;
            }
            else
            {
                task.Count += 5;
            }
        }
        public static void ChangeLevelByMoneySpent(UserDataModel user)
        {
            if (user.MoneySpent >= 300)
            {
                user.Level = 3;
            }
            else if (user.MoneySpent >= 100)
            {
                user.Level = 2;
            }
        }
        /// <summary>
        /// This is called when the user is one of the 'avoiden' ones,
        /// so they get only the minimum (25$) of their people's transactions
        /// </summary>
        /// <param name="balance">Whole balance of the user</param>
        /// <param name="users">Users he invited</param>
        public static void AddBalanceByUserCount(ref decimal balance, List<UserDataModel> users)
        {
            balance += 25 * users.Count(x => x.MoneySpent >= 25);
        }
        public static void AddBalanceByUserMoney(ref decimal balance, List<UserDataModel> users)
        {
            balance += users.Select(x => x.MoneySpent).Sum();
        }
    }
}
