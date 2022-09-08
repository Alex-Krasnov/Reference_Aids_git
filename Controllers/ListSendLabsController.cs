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
    public class ListSendLabsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ListSendLabsController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "ListSendLabs";
            return View("Index", await _context.ListSendLabs.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("SendLabId,SendLabName")] ListSendLab list)
        {
            if (ModelState.IsValid && await _context.ListSendLabs.FindAsync(list.SendLabId) == null)
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var list = await _context.ListSendLabs.FindAsync(id);

            if (list != null)
            {
                _context.ListSendLabs.Remove(list);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("SendLabId,SendLabName")] ListSendLab list)
        {
            if (ModelState.IsValid) 
            {
                _context.ListSendLabs.Update(list);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
