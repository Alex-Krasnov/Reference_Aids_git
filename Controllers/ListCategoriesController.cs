using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class ListCategoriesController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ListCategoriesController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "ListCategories";
            return View("Index", await _context.ListCategories.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] ListCategory list)
        {
            if (ModelState.IsValid && await _context.ListCategories.FindAsync(list.CategoryId) == null)
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id) 
        {
            var ListCategory = await _context.ListCategories.FindAsync(id);

            if (ListCategory != null)
            {
                _context.ListCategories.Remove(ListCategory);
            }
            
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("CategoryId,CategoryName")] ListCategory list)
        {
            if (ModelState.IsValid) //&& await _context.ListCategories.FindAsync(listCategory.CategoryId) != null
            {
                _context.ListCategories.Update(list);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction("Index");
        }
    }
}
