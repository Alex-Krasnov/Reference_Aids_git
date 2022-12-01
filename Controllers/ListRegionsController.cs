using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class ListRegionsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ListRegionsController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "ListRegions";
            return View("Index", await _context.ListRegions.OrderBy(e => e.RegionId).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("RegionId,RegionName")]  ListRegion list)
        {
            if (await _context.ListRecForRpts.FindAsync(list.RegionId) == null || list.RegionId == null) //ModelState.IsValid && 
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var List = await _context.ListRegions.FindAsync(id);

            if (List != null)
            {
                _context.ListRegions.Remove(List);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("RegionId,RegionName")] ListRegion list)
        {
            if (ModelState.IsValid) 
            {
                _context.ListRegions.Update(list);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
