// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using GrabDemSite.Data;
using Microsoft.EntityFrameworkCore;
using GrabDemSite.Models.DataModel;
using GrabDemSite.Constants;
namespace GrabDemSite.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private static int count = 100000;
        private readonly SignInManager<UserDataModel> _signInManager;
        private readonly UserManager<UserDataModel> _userManager;
        private readonly IUserStore<UserDataModel> _userStore;
        private readonly IUserEmailStore<UserDataModel> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private ApplicationDbContext _context;
        public RegisterModel(
            UserManager<UserDataModel> userManager,
            IUserStore<UserDataModel> userStore,
            SignInManager<UserDataModel> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required]
            [Display(Name = "Username")]
            [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Required]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]

            [Display(Name = "Invite code")]
            public string InviteWithLink { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                count += Random.Shared.Next(1, 500);
                var user = CreateUser();
                if(await _context.Users.AnyAsync(x => x.UserName == Input.UserName))
                {
                    ViewData["ErrorU"] = "Username already in use";
                    return Page();
                }
                if(await _context.Users.AnyAsync(x => x.Email == Input.Email))
                {
                    ViewData["ErrorE"] = "Email already in use";
                    return Page();
                }
                user.Id = Guid.NewGuid().ToString();
                user.InviteLink = count.ToString();
                user.DateCreated = DateTimeOffset.Now;
                user.MoneySpent = 0.00m;
                user.Balance = 0.00m;
                user.WalletAddress = "";
                user.InviteCount = 0;
                user.PlayMoney = 0;
                user.Level = 1;
                user.InviteWithLink = Input.InviteWithLink;
                if (Input.UserName != ConstantsVars.adminName)
                {
                    if (!await _context.Users.AnyAsync(x => x.InviteLink == Input.InviteWithLink))
                    {
                        throw new Exception("Invalid Invite code");
                    }
                    else
                    {
                        UserDataModel user1 = await _context.Users.SingleAsync(x => x.InviteLink == Input.InviteWithLink);
                        user1.InviteCount++;
                        _context.Update(user1);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    user.InviteWithLink = "";
                }
                user.EmailConfirmed = true;
                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                TaskDataModel task = new TaskDataModel(Guid.NewGuid().ToString(),0,user,1,true);
                _context.TaskDatas.Add(task);
                if (result.Succeeded)
                {
                    _context.SaveChanges();
                    _logger.LogInformation("User created a new account with password.");
                    ViewData["Error"] = null;
                        return RedirectToAction("Index");
                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private UserDataModel CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UserDataModel>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserDataModel)}'. " +
                    $"Ensure that '{nameof(UserDataModel)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<UserDataModel> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<UserDataModel>)_userStore;
        }
    }
}
