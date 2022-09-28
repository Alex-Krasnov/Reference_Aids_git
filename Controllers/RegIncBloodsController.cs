using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;
using Reference_Aids.ModelsForInput;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class RegIncBloodsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public RegIncBloodsController(Reference_AIDSContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var Viewdata = new ListForPatientCardViewModel
            {
                ListSexes = await _context.ListSexes.ToListAsync(),
                ListRegions = await _context.ListRegions.ToListAsync(),
                ListSendLabs = await _context.ListSendLabs.ToListAsync(),
                ListTestSystems = await _context.ListTestSystems.ToListAsync(),
                TblPatientCards = await _context.TblPatientCards.Where(e => e.PatientId == id).ToListAsync(),
                TblDistrictBlots = await _context.TblDistrictBlots.Where(e => e.PatientId == id).ToListAsync(),
                TblIncomingBloods = await _context.TblIncomingBloods.Where(e => e.PatientId == id).ToListAsync(),
                ListSendDistricts = await _context.ListSendDistricts.ToListAsync(),
                ListCategories = await _context.ListCategories.ToListAsync(),
                ListQualitySerums = await _context.ListQualitySerums.ToListAsync()
            };
            ViewBag.Title = "InputIncBlood";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDistrictBlot(InputDistricBlot list)  
        {
            if (ModelState.IsValid)
            {
                
                TblDistrictBlot tblDistrictBlot = new()
                {
                    PatientId = list.PatientId,
                    DBlot = list.DBlot,
                    CutOff = list.CutOff,
                    BlotResult = list.BlotResult,
                    BlotCoefficient = Math.Round((list.BlotResult / list.CutOff), 3),
                    TestSystemId = list.TestSystemId,
                    SendDistrictId = list.SendDistrictId,
                    SendLabId = list.SendLabId
                };

                _context.TblDistrictBlots.Add(tblDistrictBlot);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = list.PatientId });
            }
            return RedirectToAction("Index", new { id = list.PatientId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateIncomingBlot(InputIncomingBlood list)
        {
            if (ModelState.IsValid)
            {
                TblIncomingBlood tblIncomingBlood = new()
                {
                    PatientId = list.PatientId,
                    SendDistrictId = list.SendDistrictId(_context),
                    SendLabId = list.SendLabId(_context),
                    CategoryPatientId = list.CategoryPatientId(_context),
                    AnonymousPatient = list.AnonymousPatient,
                    DateBloodSampling = DateOnly.Parse(list.DateBloodSampling),
                    QualitySerumId = list.QualitySerumId(_context),
                    DateBloodImport = DateOnly.Parse(list.DateBloodImport),
                    NumIfa = list.NumIfa,
                    NumInList = list.NumInList
                };
                _context.TblIncomingBloods.Add(tblIncomingBlood);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = list.PatientId });
            }
            return RedirectToAction("Index", new { id = list.PatientId });
        }
    }
}
