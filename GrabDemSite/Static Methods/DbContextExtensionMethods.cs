using GrabDemSite.Data;
using GrabDemSite.Models;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Extension_methods
{
    public static class DbContextExtensionMethods
    {
        public static UserDataModel GetUserById(this ApplicationDbContext context,string id)
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
        public static UserDataModel GetUserByName(this ApplicationDbContext context, string name) {
            return context.Users.Where(x => x.UserName == name).Single();
        }
        public static List<WithdrawDataModel> GetWithdrawsByUser(this ApplicationDbContext context, UserDataModel user) {
            return context.WithdrawDatas.Where(x => x.User == user).ToList();
        }
    }
}
