using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;
using Reference_Aids.ModelsForInput;
using Reference_Aids.ViewModels;
using System.Data;

namespace Reference_Aids.Controllers
{
    public class ImportBlotController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public ImportBlotController(Reference_AIDSContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int countRow)
        {
            var Viewdata = new ListForImportBlotViewModel
            {
                ListTestSystems = await _context.ListTestSystems.ToListAsync(),
                CountRow = countRow
            };
            ViewBag.Title = "ImportBlot";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> Add(List<InputBlot> list)
        {
            if (ModelState.IsValid)
            {
                DateOnly dateNow = DateOnly.FromDateTime(DateTime.Today);
                foreach (var item in list)
                {
                    TblResultBlot tblResultBlot = new()
                    {
                        //BloodId = _context.TblIncomingBloods.Where(e => e.NumIfa == item.PatientId && e.DateBloodImport.Year == dateNow.Year).First().BloodId,
                        //ResultBlotDate = dateNow,
                        //ExpirationResultBlotDate = DateOnly.Parse(item.ExpirationResultBlotDate),
                        //ResultBlotTestSysId = item.TestSysId(_context),
                        //ResultBlotEnv160 = item.ResultBlotEnv160Id(_context),
                        //ResultBlotEnv120 = item.ResultBlotEnv120Id(_context),
                        //ResultBlotEnv41 = item.ResultBlotEnv41Id(_context),
                        //ResultBlotGag55 = item.ResultBlotGag55Id(_context),
                        //ResultBlotGag40 = item.ResultBlotGag40Id(_context),
                        //ResultBlotGag2425 = item.ResultBlotGag2425Id(_context),
                        //ResultBlotGag18 = item.ResultBlotGag18Id(_context),
                        //ResultBlotPol6866 = item.ResultBlotPol6866Id(_context),
                        //ResultBlotPol5251 = item.ResultBlotPol5251Id(_context),
                        //ResultBlotPol3431 = item.ResultBlotPol3431Id(_context),
                        //ResultBlotHiv2105 = item.ResultBlotHiv2105Id(_context),
                        //ResultBlotHiv236 = item.ResultBlotHiv236Id(_context),
                        //ResultBlotHiv0 = item.ResultBlotHiv0Id(_context),
                        //ResultBlotReturnResult = item.ResultBlotReturnResult,
                        //ResultBlotResultId = item.ResultId(_context)
                    };

                    //_context.TblResultBlots.Add(tblResultBlot);
                    //await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "ImportAnalyzes");
            }
            return RedirectToAction("Index", "ImportAnalyzes");
        }
    }
}
