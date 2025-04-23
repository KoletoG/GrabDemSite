using GrabDemSite.Models.DataModel;

namespace GrabDemSite.Models.ViewModel
{
    public class AdminWithdrawConfirmViewModel
    {
        public WithdrawDataModel[] Withdraws { get; private init; }
        public AdminWithdrawConfirmViewModel(WithdrawDataModel[] withdraw)
        {
            Withdraws = withdraw;
        }
    }
}
