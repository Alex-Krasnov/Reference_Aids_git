using Microsoft.AspNetCore.Mvc;

namespace Reference_Aids.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(List<string> list)
        {
            return View(list);
        }
    }
}
