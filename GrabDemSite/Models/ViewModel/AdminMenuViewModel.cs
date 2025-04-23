using GrabDemSite.Models.DataModel;

namespace GrabDemSite.Models.ViewModel
{
    internal class AdminMenuViewModel
    {
        internal DepositDataModel[] Deposits { get; private init; }
        internal WithdrawDataModel[] Withdraws { get; private init; }
        internal UserDataModel[] Users { get; private init; }
        internal AdminMenuViewModel(DepositDataModel[] deposits, WithdrawDataModel[] withdraws, UserDataModel[] users) 
        {
            Deposits = deposits;
            Withdraws = withdraws;
            Users = users;
        }
    }
}
