using System.ComponentModel.DataAnnotations;

namespace GrabDemSite.Models
{
    public class DepositDataModel
    {
        [Key]
        public string Id { get; set; }
        public UserDataModel User { get; set; }
        public string UserEmail { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public double MoneyForDeposit { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
