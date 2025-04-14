using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;

namespace Reference_Aids.Controllers
{
    public class ReportController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ReportController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Report";
            return View("Index", _context.ListRecForRpts.ToList());
        }
    }
}
