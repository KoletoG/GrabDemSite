using Microsoft.AspNetCore.Mvc;

namespace GrabDemSite.Controllers
{
    public class TasksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
