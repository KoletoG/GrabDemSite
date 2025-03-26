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
        public static List<DepositDataModel> GetDepositsByUser(this ApplicationDbContext context, UserDataModel user)
        {
            return context.DepositDatas.Where(x => x.User == user).ToList();
        }
        public static List<DepositDataModel> GetDepositsByUserAndIsConfirmed(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = true)
        {
            return context.DepositDatas.Where(x => x.User == user && x.IsConfirmed==isConfirmed).ToList();
        }
        public static List<DepositDataModel> GetDepositsByUserIdAndIsConfirmed(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = false)
        {
            return context.DepositDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == isConfirmed).ToList();
        }
        public static UserDataModel GetUserByName(this ApplicationDbContext context, string name)
        {
            return context.Users.Where(x => x.UserName == name).Single();
        }
        public static List<WithdrawDataModel> GetWithdrawsByUser(this ApplicationDbContext context, UserDataModel user)
        {
            return context.WithdrawDatas.Where(x => x.User == user).ToList();
        }
        public static List<WithdrawDataModel> GetWithdrawsByWalletAndIsConfirmed(this ApplicationDbContext context, string wallet, bool isConfirmed=false)
        {
            return context.WithdrawDatas.Where(x => x.WalletAddress == wallet && x.IsConfirmed==isConfirmed).ToList();
        }
        public static List<WithdrawDataModel> GetWithdrawsByUserIdAndIsConfirmed(this ApplicationDbContext context, UserDataModel user, bool isConfirmed = false)
        {
            return context.WithdrawDatas.Where(x => x.User.Id == user.Id && x.IsConfirmed == isConfirmed).ToList();
        }
        public static UserDataModel GetUserByInviteLink(this ApplicationDbContext context, UserDataModel user)
        {
            return context.Users.Where(x => x.InviteLink == user.InviteWithLink).Single();
        }
        /*
         for (int i = 0; i < userslv2.Count(); i++)
                        {
                            userslv3.AddRange(_context.Users.Where(x => x.InviteWithLink == userslv2[i].InviteLink).ToList());
                        }
         */
        public static void AddUsersToTeamByLevel(this ApplicationDbContext context, ref List<UserDataModel> users1, ref List<UserDataModel> users2, UserDataModel fakeUser)
        {
            UserDataModel user = new UserDataModel();
            users2.Remove(fakeUser);
            for (int i = 0; i < users1.Count(); i++)
            {
                user = users1[i];
                users2.AddRange(context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToList());
            }
            users2.OrderBy(x => x.MoneySpent);
        }
        public static void AddUsersToTeamByLevel(this ApplicationDbContext context, ref List<UserDataModel> users, UserDataModel user, UserDataModel fakeUser)
        {
            users.Remove(fakeUser);
            users = context.Users.Where(x => x.InviteWithLink == user.InviteLink).ToList();
            users.OrderBy(x => x.MoneySpent);
        }
        /*
         _context.Users.Where(x => x.InviteWithLink == userslv1[0].InviteLink).FirstOrDefault() != default
         */
        public static bool IsInviteLinkUsersExist(this ApplicationDbContext context, List<UserDataModel> users) 
        {
            if(context.Users.Where(x => x.InviteWithLink == users[0].InviteLink).FirstOrDefault() != default)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsInviteLinkUsersExist(this ApplicationDbContext context, UserDataModel user)
        {
            if (context.Users.Where(x => x.InviteWithLink == user.InviteLink).FirstOrDefault() != default)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
