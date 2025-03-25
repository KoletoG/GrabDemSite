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
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
        public DepositDataModel() { }
        public DepositDataModel(string id, UserDataModel user, string userEmail, double moneyForDeposit, bool isConfirmed, DateTime dateCreated)
        {
            Id = id;
            User = user;
            UserEmail = userEmail;
            MoneyForDeposit = moneyForDeposit;
            IsConfirmed = isConfirmed;
            DateCreated = dateCreated;
        }
        public DepositDataModel(string id,UserDataModel user, string userEmail,double moneyForDeposit)
        {
            Id = id; 
            User = user;
            UserEmail = userEmail; 
            MoneyForDeposit = moneyForDeposit;
        }
    }
}
