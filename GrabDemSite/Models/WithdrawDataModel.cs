using System.ComponentModel.DataAnnotations;

namespace GrabDemSite.Models
{
    public class WithdrawDataModel
    {
        [Key]
        public string Id { get; set; }
        public string WalletAddress { get; set; }
        [DataType(DataType.Currency)]
        public double Money { get; set; }
        public UserDataModel User { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime DateCreated { get; set; }

        public WithdrawDataModel(string id, string walletAddress,double money, UserDataModel user, bool isConfirmed, DateTime dateCreated)
        {
            this.Id = id;
            this.WalletAddress = walletAddress;
            this.Money = money;
            this.User = user;
            this.IsConfirmed = isConfirmed;
            this.DateCreated = dateCreated;
        }
        public WithdrawDataModel(string id, string walletAddress, double money, UserDataModel user)
        {
            this.Id = id;
            this.WalletAddress = walletAddress;
            this.Money = money;
            this.User = user;
        }
        public WithdrawDataModel() { }
    }
}
