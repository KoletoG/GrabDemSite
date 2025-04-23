using GrabDemSite.Models.DataModel;

namespace GrabDemSite.Models.ViewModel
{
    public class EditViewModel
    {
        public UserDataModel User { get; private init; }
        public DepositDataModel[] Deposits { get; private init; }
        public List<WithdrawDataModel> Withdraws { get; private init; }
        public EditViewModel(UserDataModel user, DepositDataModel[] deposits, List<WithdrawDataModel> withdraws)
        {
            User = user;
            Deposits = deposits;
            Withdraws = withdraws;
        }
    }
}
