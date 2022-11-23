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
    public class ListSendDistrictsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ListSendDistrictsController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "ListSendDistricts";
            return View("Index", await _context.ListSendDistricts.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("SendDistrictId,SendDistrictName")] ListSendDistrict list)
        {
            if (ModelState.IsValid && await _context.ListSendDistricts.FindAsync(list.SendDistrictId) == null)
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var list = await _context.ListSendDistricts.FindAsync(id);

            if (list != null)
            {
                _context.ListSendDistricts.Remove(list);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("SendDistrictId,SendDistrictName")] ListSendDistrict list)
        {
            if (ModelState.IsValid)
            {
                _context.ListSendDistricts.Update(list);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}

