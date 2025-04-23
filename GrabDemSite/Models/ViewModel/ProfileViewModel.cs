using System.Net;
using GrabDemSite.Models.DataModel;

namespace GrabDemSite.Models.ViewModel
{
    public class ProfileViewModel
    {
        public UserDataModel User { get; private init; }
        public List<UserDataModel> userslv1 {  get; private init; }
        public List<UserDataModel> userslv2 { get; private init; }
        public List<UserDataModel> userslv3 { get; private init; }
        public decimal WholeBal {  get; private init; }
        public DepositDataModel[] DepositOrders { get; private init; }
        public List<WithdrawDataModel> WithdrawOrders { get; private init; }
        public ProfileViewModel(List<UserDataModel> lv1, List<UserDataModel> lv2, List<UserDataModel> lv3, decimal wholeBal, DepositDataModel[] deposits, List<WithdrawDataModel> withdraws, UserDataModel user)
        {
            userslv1 = lv1;
            userslv2 = lv2;
            userslv3 = lv3;
            WholeBal = wholeBal;
            DepositOrders = deposits;
            WithdrawOrders = withdraws;
            User = user;
        }
    }
}
