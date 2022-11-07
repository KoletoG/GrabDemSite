using GrabDemSite.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace GrabDemSite.Views.Home
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<UserDataModel> _signInManager;
        private readonly UserManager<UserDataModel> _userManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailSender _emailSender;
        public IndexModel(
            UserManager<UserDataModel> userManager,
            SignInManager<UserDataModel> signInManager,
            ILogger<IndexModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        public string? Title { get; set; } = "Test";
    }
}
