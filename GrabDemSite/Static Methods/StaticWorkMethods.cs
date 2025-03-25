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
        public static void AddBalanceByUserCount(ref double balance,List<UserDataModel> users)
        {
            foreach (var user in users)
            {
                if (user.MoneySpent >= 25)
                {
                    balance += users.Count() * 25;
                }
            }
        }
        public static void AddBalanceByUserMoney(ref double balance,List<UserDataModel> users)
        {
            for (int i = 0; i < users.Count(); i++)
            {
                balance += users[i].MoneySpent;
            }
        }
        /*
         for (int i = 0; i < userslv3.Count(); i++)
                            {
                                wholeBal += userslv3[i].MoneySpent;
                            }
         */
        /*
          foreach (var user11 in userslv2)
                        {
                            if (user11.MoneySpent >= 25)
                            {
                                wholeBal += userslv1.Count() * 25;
                            }
                        }
         */
    }
}
