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
            List<string> ErrList = new();
            DateOnly dateNow = DateOnly.FromDateTime(DateTime.Today);
            int i = 1;
            foreach (var item in list)
            {
                try
                {
                    int? res160 = null, res120 = null, res41 = null, res55 = null, res40 = null, res2425 = null, res18 = null, res6866 = null, res5251 = null,
                        res3431 = null, res2105 = null, res236 = null, res0 = null;
                    try { res160 = _context.ListResults.First(e => e.ResultName == item.ResultBlotEnv160).ResultId; } catch { }
                    try { res120 = _context.ListResults.First(e => e.ResultName == item.ResultBlotEnv120).ResultId; } catch { }
                    try { res41 = _context.ListResults.First(e => e.ResultName == item.ResultBlotEnv41).ResultId; } catch { }
                    try { res55 = _context.ListResults.First(e => e.ResultName == item.ResultBlotGag55).ResultId; } catch { }
                    try { res40 = _context.ListResults.First(e => e.ResultName == item.ResultBlotGag40).ResultId; } catch { }
                    try { res2425 = _context.ListResults.First(e => e.ResultName == item.ResultBlotGag2425).ResultId; } catch { }
                    try { res18 = _context.ListResults.First(e => e.ResultName == item.ResultBlotGag18).ResultId; } catch { }
                    try { res6866 = _context.ListResults.First(e => e.ResultName == item.ResultBlotPol6866).ResultId; } catch { }
                    try { res5251 = _context.ListResults.First(e => e.ResultName == item.ResultBlotPol5251).ResultId; } catch { }
                    try { res3431 = _context.ListResults.First(e => e.ResultName == item.ResultBlotPol3431).ResultId; } catch { }
                    try { res2105 = _context.ListResults.First(e => e.ResultName == item.ResultBlotHiv2105).ResultId; } catch { }
                    try { res236 = _context.ListResults.First(e => e.ResultName == item.ResultBlotHiv236).ResultId; } catch { }
                    try { res0 = _context.ListResults.First(e => e.ResultName == item.ResultBlotHiv0).ResultId; } catch { }

                    TblResultBlot tblResultBlot = new()
                    {
                        BloodId = _context.TblIncomingBloods.Where(e => e.NumIfa == item.BloodId && e.DateBloodImport.Year == dateNow.Year).First().BloodId,
                        ResultBlotDate = dateNow,
                        ExpirationResultBlotDate = DateOnly.Parse(item.ExpirationResultBlotDate),
                        ResultBlotTestSysId = _context.ListTestSystems.Where(e => e.TestSystemName == item.ResultBlotTestSysName).First().TestSystemId,
                        ResultBlotEnv160 = res160,
                        ResultBlotEnv120 = res120,
                        ResultBlotEnv41 = res41,
                        ResultBlotGag55 = res55,
                        ResultBlotGag40 = res40,
                        ResultBlotGag2425 = res2425,
                        ResultBlotGag18 = res18,
                        ResultBlotPol6866 = res6866,
                        ResultBlotPol5251 = res5251,
                        ResultBlotPol3431 = res3431,
                        ResultBlotHiv2105 = res2105,
                        ResultBlotHiv236 = res236,
                        ResultBlotHiv0 = res0,
                        ResultBlotResultId = _context.ListResults.Where(e => e.ResultName == item.ResultBlotResult).First().ResultId
                    };

                    _context.TblResultBlots.Add(tblResultBlot);
                    await _context.SaveChangesAsync();
                }
                catch 
                {
                    ErrList.Add($"Ошибка в строке №{i}");
                }
                i++;
            }
            
            if (ErrList != null)
                return RedirectToAction("Index", "Error", new {list = ErrList });

            return RedirectToAction("Index", "ImportAnalyzes");
        }
    }
}
