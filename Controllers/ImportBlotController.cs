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
        public async Task<IActionResult> Index(int countRow, string testSystem, string date)
        {
            var Viewdata = new ListForImportBlotViewModel
            {
                ListTestSystems = await _context.ListTestSystems.ToListAsync(),
                CountRow = countRow, 
                Date = date,
                TestSystem = testSystem
            };
            ViewBag.Title = "ImportBlot";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> Add(List<InputBlot> list)
        {
            DateOnly dateNow = DateOnly.FromDateTime(DateTime.Today);
            foreach (var item in list)
            {
                try
                {
                    TblResultBlot tblResultBlot = new()
                    {
                        BloodId = _context.TblIncomingBloods.Where(e => e.NumIfa == item.BloodId && e.DateBloodImport.Year == dateNow.Year).First().BloodId,
                        ResultBlotDate = dateNow,
                        ExpirationResultBlotDate = DateOnly.Parse(item.ExpirationResultBlotDate),
                        ResultBlotTestSysId = _context.ListTestSystems.Where(e => e.TestSystemName == item.ResultBlotTestSysName).First().TestSystemId,
                        ResultBlotEnv160 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotEnv160).First().ResultId,
                        ResultBlotEnv120 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotEnv120).First().ResultId,
                        ResultBlotEnv41 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotEnv41).First().ResultId,
                        ResultBlotGag55 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotGag55).First().ResultId,
                        ResultBlotGag40 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotGag40).First().ResultId,
                        ResultBlotGag2425 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotGag2425).First().ResultId,
                        ResultBlotGag18 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotGag18).First().ResultId,
                        ResultBlotPol6866 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotPol6866).First().ResultId,
                        ResultBlotPol5251 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotPol5251).First().ResultId,
                        ResultBlotPol3431 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotPol3431).First().ResultId,
                        ResultBlotHiv2105 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotHiv2105).First().ResultId,
                        ResultBlotHiv236 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotHiv236).First().ResultId,
                        ResultBlotHiv0 = _context.ListResults.Where(e => e.ResultName == item.ResultBlotHiv0).First().ResultId,
                        ResultBlotResultId = _context.ListResults.Where(e => e.ResultName == item.ResultBlotResult).First().ResultId
                    };

                    _context.TblResultBlots.Add(tblResultBlot);
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
