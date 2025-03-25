using GrabDemSite.Controllers;
using GrabDemSite.Data;
using GrabDemSite.Models;
using Microsoft.EntityFrameworkCore;

namespace GrabDemSite.Methods
{
    public class MethodsCall
    {
        private ApplicationDbContext Context { get; set; }
        private HomeController homeController { get; set; }
        public MethodsCall(ApplicationDbContext _context,HomeController _homeController)
        {
            Context = _context;
            homeController = _homeController;
        }

        public UserDataModel GetUser()
        {
            var current = Context.Users.Where(x => x.UserName == homeController.User.Identity.Name).Single();
            return current;
        }
    }
}
