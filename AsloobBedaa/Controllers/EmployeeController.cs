using Microsoft.AspNetCore.Mvc;

namespace AsloobBedaa.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
