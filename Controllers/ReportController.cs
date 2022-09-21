using Microsoft.AspNetCore.Mvc;

namespace Reference_Aids.Controllers
{
    public class ReportController : Controller
    {
        [HttpGet]
        public IActionResult Form4routing()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ReportAnalyzes()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Report2()
        {
            return View();
        }
    }
}
