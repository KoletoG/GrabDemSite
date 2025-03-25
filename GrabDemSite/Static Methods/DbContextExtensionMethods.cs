using GrabDemSite.Data;
using GrabDemSite.Models;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Extension_methods
{
    public static class DbContextExtensionMethods
    {
        public static UserDataModel GetUserById(this ApplicationDbContext context, string id)
        {
            return context.Users.Where(x => x.Id == id).Single();
        }

        public static TaskDataModel GetTaskByUser(this ApplicationDbContext context, UserDataModel user)
        {
            return context.TaskDatas.Where(x => x.User == user).Single();
        }
        public static List<DepositDataModel> GetDepositByUser(this ApplicationDbContext context, UserDataModel user)
        {
            return context.DepositDatas.Where(x => x.User == user).ToList();
        }
        public static UserDataModel GetUserByName(this ApplicationDbContext context, string name)
        {
            return context.Users.Where(x => x.UserName == name).Single();
        }
        public static List<WithdrawDataModel> GetWithdrawsByUser(this ApplicationDbContext context, UserDataModel user)
        {
            return context.WithdrawDatas.Where(x => x.User == user).ToList();
        }
        /*
         for (int i = 0; i < userslv2.Count(); i++)
                        {
                            userslv3.AddRange(_context.Users.Where(x => x.InviteWithLink == userslv2[i].InviteLink).ToList());
                        }
         */
        public static void AddUsersToTeamByLevel(this ApplicationDbContext context, ref List<UserDataModel> users1, ref List<UserDataModel> users2)
        {
            UserDataModel user = new UserDataModel();
            for (int i = 0; i < users1.Count(); i++)
            {
                user = users1[i];
                users2.AddRange(context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToList());
            }
            users2.OrderBy(x => x.MoneySpent);
        }
        public static void AddUsersToTeamBy(this ApplicationDbContext context, ref List<UserDataModel> users, UserDataModel user)
        {
            users = context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToList();
            users.OrderBy(x => x.MoneySpent);
        }
    }
}
