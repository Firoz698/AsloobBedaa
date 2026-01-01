using Microsoft.AspNetCore.Mvc;

namespace AsloobBedaa.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
