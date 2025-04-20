using GrabDemSite.Models.DataModel;

namespace GrabDemSite.Models.ViewModel
{
    public class AdminWithdrawConfirmViewModel
    {
        public List<WithdrawDataModel> Withdraws { get; private init; }
        public AdminWithdrawConfirmViewModel(List<WithdrawDataModel> withdraw)
        {
            Withdraws = withdraw;
        }
    }
}
