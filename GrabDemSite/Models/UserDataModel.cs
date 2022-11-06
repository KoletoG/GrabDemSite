using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GrabDemSite.Models
{
    public class UserDataModel : IdentityUser
    {
        [Key]
        public override string Id { get; set; }
        [Required(AllowEmptyStrings =false,ErrorMessage ="Username cannot be empty.")]
        [StringLength(25, MinimumLength = 3)]
        public override string UserName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public override string Email { get; set; }
        public string InviteLink { get; set;}
        [DataType(DataType.DateTime)]
        public DateTimeOffset DateCreated { get; set; }
        [DataType(DataType.Currency)]
        public double Balance { get; set; }
        [DataType(DataType.Currency)]
        public double MoneySpent { get; set; }
        public string WalletAddress { get; set;}
        public int Level { get; set; }
    }
}
