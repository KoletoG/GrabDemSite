using System.ComponentModel.DataAnnotations;
using GrabDemSite.Interfaces;

namespace GrabDemSite.Models.DataModel
{
    public class WithdrawDataModel : ITransactionDataModel
    {
        [Key]
        public string Id { get; set; }
        public string WalletAddress { get; set; }
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }
        public UserDataModel User { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime DateCreated { get; set; }

        public WithdrawDataModel(string id, string walletAddress, decimal money, UserDataModel user, bool isConfirmed, DateTime dateCreated)
        {
            Id = id;
            WalletAddress = walletAddress;
            Money = money;
            User = user;
            IsConfirmed = isConfirmed;
            DateCreated = dateCreated;
        }
        public WithdrawDataModel(string id, string walletAddress, decimal money, UserDataModel user)
        {
            Id = id;
            WalletAddress = walletAddress;
            Money = money;
            User = user;
        }
        public WithdrawDataModel() { }
    }
}
