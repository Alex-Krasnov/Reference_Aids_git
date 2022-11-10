using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;
using Reference_Aids.ModelsForInput;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class ImportPcrController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public ImportPcrController(Reference_AIDSContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int numIfaStart, int  numIfaEnd, string testSystem)
        {
            var Viewdata = new ListForImportPcr
            {
                ListTestSystems = await _context.ListTestSystems.ToListAsync(),
                ListResults = await _context.ListResults.ToListAsync(),
                IfaStart = numIfaStart,
                IfaEnd = numIfaEnd,
                TestSystemName = testSystem
            };

            ViewBag.Title = "ImportPcr";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> Add(List<InputPcr> list)
        {
            DateOnly dateNow = DateOnly.FromDateTime(DateTime.Today);
            foreach (var item in list)
            {
                if (item.BloodId == null)
                    continue;

                try
                {
                    TblResultPcr tblResultPcr = new()
                    {
                        BloodId = _context.TblIncomingBloods.Where(e => e.NumIfa == item.BloodId && e.DateBloodImport.Year == dateNow.Year).First().BloodId,
                        ResultPcrDate = dateNow,
                        ResultPcrTestSysId = _context.ListTestSystems.Where(e => e.TestSystemName == item.ResultPcrTestSysName).First().TestSystemId,
                        ResultPcrResultId = _context.ListResults.Where(e => e.ResultName == item.ResultPcrResultName).First().ResultId
                    };

                    _context.TblResultPcrs.Add(tblResultPcr);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    //return RedirectToAction("Error", "ImportBlot", new { });
                }
            }
            return RedirectToAction("Index", "ImportAnalyzes");
        }
    }
}
