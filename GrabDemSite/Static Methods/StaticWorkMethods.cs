using GrabDemSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace GrabDemSite.Static_Methods
{
    public static class StaticWorkMethods
    {
        public static double AddBalanceByLevel(double balance, double playMoney, int level)
        {
            switch (level)
            {
                case 1:balance += playMoney * 0.05;break;
                case 2:balance += playMoney * 0.06;break;
                case 3: balance += playMoney * 0.07;break;
            }
            return balance;
        }
        public static void IncreaseTaskAndBalance(double balance, ref TaskDataModel task, ref UserDataModel user)
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
        public static void ChangeLevelByMoneySpent(ref UserDataModel user)
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
    }
}
