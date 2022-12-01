using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class ListQualitySerumsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ListQualitySerumsController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "ListQualitySerums";
            return View("Index", await _context.ListQualitySerums.OrderBy(e => e.QualitySerumId).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("QualitySerumId,QualitySerumName")]  ListQualitySerum list)
        {
            if (ModelState.IsValid && await _context.ListQualitySerums.FindAsync(list.QualitySerumId) == null)
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var List = await _context.ListQualitySerums.FindAsync(id);

            if (List != null)
            {
                _context.ListQualitySerums.Remove(List);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("QualitySerumId,QualitySerumName")] ListQualitySerum list)
        {
            if (ModelState.IsValid) 
            {
                _context.ListQualitySerums.Update(list);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
