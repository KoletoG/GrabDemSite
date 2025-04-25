using GrabDemSite.Models.DataModel;
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
        public static decimal CalculateBalance(UserDataModel user, decimal balance, int count = 1, bool nameToAvoid = false)
        {
            if (count > 3)
            {
                return balance;
            }
            if (nameToAvoid)
            {
                balance += user.InvitedUsers.Count * 25;
            }
            else
            {
                balance += user.InvitedUsers.Sum(x => x.MoneySpent);
            }
            count++;
            foreach (var user1 in user.InvitedUsers)
            {
                balance += CalculateBalance(user1, balance, count, nameToAvoid);
            }
            return balance;
        }
        public static void LoadUserLevels(List<UserDataModel> lv1, List<UserDataModel> lv2, List<UserDataModel> lv3, UserDataModel user, int count = 1)
        {
            if (count == 1)
            {
                lv1 = user.InvitedUsers.ToList();
                foreach (var user1 in lv1)
                {
                    LoadUserLevels(lv1, lv2, lv3, user, 2);
                }
            }
            else if (count == 2)
            {
                lv2.AddRange(user.InvitedUsers.ToList());
                foreach (var user1 in lv2)
                {
                    LoadUserLevels(lv1, lv2, lv3, user, 3);
                }
            }
            else if (count == 3)
            {
                lv3.AddRange(user.InvitedUsers.ToList());
            }
        }
    }
}
