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
    }
}
