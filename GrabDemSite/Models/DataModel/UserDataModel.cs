﻿using GrabDemSite.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GrabDemSite.Models.DataModel
{
    public class UserDataModel : IdentityUser, IUserDataModel
    {
        public UserDataModel(string id, string userName, string email, string inviteLink, string inviteWithLink, DateTimeOffset dateCreated, decimal balance, decimal moneySpent, string walletAddress, short inviteCount, decimal playMoney, byte level)
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
        public UserDataModel InviteByUser { get; set; }
        public List<UserDataModel> InvitedUsers { get; set; }
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
        public decimal Balance { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Money spent by user: ")]
        public decimal MoneySpent { get; set; }
        [Display(Name = "User's wallet address: ")]
        public string WalletAddress { get; set; }
        public short InviteCount { get; set; }
        public decimal PlayMoney { get; set; }
        public byte Level { get; set; }
    }
}
