using Microsoft.AspNetCore.Mvc;

namespace Reference_Aids.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}