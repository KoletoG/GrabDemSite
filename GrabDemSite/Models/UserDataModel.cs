using GrabDemSite.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GrabDemSite.Models
{
    public class UserDataModel : IdentityUser, IUserDataModel
    {
        public UserDataModel() { }
        public UserDataModel(string id, string userName, string email, string inviteLink, string inviteWithLink, DateTimeOffset dateCreated, double balance, double moneySpent, string walletAddress, int inviteCount, double playMoney, int level)
        {
            Id = id;
            UserName = userName;
            Email = email;
            InviteLink = inviteLink;
            InviteWithLink = inviteWithLink;
            DateCreated = dateCreated;
            Balance = balance;
            MoneySpent = moneySpent;
            WalletAddress = walletAddress;
            InviteCount = inviteCount;
            PlayMoney = playMoney;
            Level = level;
        }

        [Key]
        public override string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username cannot be empty.")]
        [StringLength(25, MinimumLength = 3)]
        [Display(Name = "User's username: ")]
        public override string UserName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public override string Email { get; set; }
        public string InviteLink { get; set; }
        [Required]
        public string InviteWithLink { get; set; }
        [DataType(DataType.DateTime)]
        public DateTimeOffset DateCreated { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Current user's balance: ")]
        public double Balance { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Money spent by user: ")]
        public double MoneySpent { get; set; }
        [Display(Name = "User's wallet address: ")]
        public string WalletAddress { get; set; }
        public int InviteCount { get; set; }
        public double PlayMoney { get; set; }
        public int Level { get; set; }
    }
}
