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
    }
}
