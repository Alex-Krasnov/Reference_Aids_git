using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;
using Reference_Aids.ModelsForInput;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class UpdatePatientCardController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public UpdatePatientCardController(Reference_AIDSContext context)
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
        public async Task<IActionResult> Update(InputRegPatient list)
        {
            if (ModelState.IsValid)
            {
                TblPatientCard tblPatientCard = new()
                {
                    PatientId = list.PatientId,
                    FamilyName = list.FamilyName,
                    FirstName = list.FirstName,
                    ThirdName = list.ThirdName,
                    BirthDate = DateOnly.Parse(list.BirthDate),
                    SexId = list.SexId(_context),
                    RegionId = list.RegionId(_context),
                    CityName = list.CityName,
                    AreaName = list.AreaName,
                    PatientCom = list.PatientCom,
                    PhoneNum = list.PhoneNum,
                    AddrHome = list.AddrHome,
                    AddrCorps = list.AddrCorps,
                    AddrFlat = list.AddrFlat,
                    AddrStreat = list.AddrStreat
                };
                _context.TblPatientCards.Update(tblPatientCard);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "RegIncBloods", new { id = tblPatientCard.PatientId });
            }
            return RedirectToAction("Index");
        }
    }
}
