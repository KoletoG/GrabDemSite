using GrabDemSite.Models.DataModel;

namespace GrabDemSite.Models.ViewModel
{
    internal class AdminMenuViewModel
    {
        internal List<DepositDataModel> Deposits { get; private init; }
        internal List<WithdrawDataModel> Withdraws { get; private init; }
        internal List<UserDataModel> Users { get; private init; }
        internal AdminMenuViewModel(List<DepositDataModel> deposits, List<WithdrawDataModel> withdraws, List<UserDataModel> users) 
        {
            Deposits = deposits;
            Withdraws = withdraws;
            Users = users;
        }
    }
}
