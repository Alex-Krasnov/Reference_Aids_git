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
    public class ListSendDepartmentsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public ListSendDepartmentsController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "ListSendDepartments";
            return View("Index", await _context.ListSendDepartments.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("SendDepartmentId,SendDepartmentName")]  ListSendDepartment list)
        {
            if (ModelState.IsValid && await _context.ListSendDepartments.FindAsync(list.SendDepartmentId) == null)
            {
                _context.Add(list);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var List = await _context.ListSendDepartments.FindAsync(id);

            if (List != null)
            {
                _context.ListSendDepartments.Remove(List);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind("SendDepartmentId,SendDepartmentName")] ListSendDepartment list)
        {
            if (ModelState.IsValid) 
            {
                _context.ListSendDepartments.Update(list);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
