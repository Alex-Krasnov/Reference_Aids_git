using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class ListTestSystemsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ListTestSystemsController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "ListTestSystems";
            return View("Index", await _context.ListTestSystems.OrderBy(e => e.TestSystemId).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("TestSystemId,TestSystemName,DTestSystemShelfLife")]  ListTestSystem list)
        {
            if (await _context.ListTestSystems.FindAsync(list.TestSystemId) == null || list.TestSystemId == null) //ModelState.IsValid && 
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var List = await _context.ListTestSystems.FindAsync(id);

            if (List != null)
            {
                _context.ListTestSystems.Remove(List);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("TestSystemId,TestSystemName,DTestSystemShelfLife")] ListTestSystem list)
        {
            if (ModelState.IsValid) 
            {
                _context.ListTestSystems.Update(list);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
