using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class ListRecForRptsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ListRecForRptsController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "ListRecForRpts";
            return View("Index", await _context.ListRecForRpts.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("RecId,RecName")]  ListRecForRpt list)
        {
            if (await _context.ListRecForRpts.FindAsync(list.RecId) == null || list.RecId == null) //ModelState.IsValid && 
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var List = await _context.ListRecForRpts.FindAsync(id);

            if (List != null)
            {
                _context.ListRecForRpts.Remove(List);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("RecId,RecName")] ListRecForRpt list)
        {
            if (ModelState.IsValid) 
            {
                _context.ListRecForRpts.Update(list);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
