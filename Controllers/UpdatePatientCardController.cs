using Microsoft.AspNetCore.Mvc;
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
                ListSexes = _context.ListSexes.ToList(),
                ListRegions = _context.ListRegions.ToList(),
                ListSendLabs = _context.ListSendLabs.ToList(),
                ListTestSystems = _context.ListTestSystems.ToList(),
                TblPatientCards = _context.TblPatientCards.Where(e => e.PatientId == id).ToList(),
                TblDistrictBlots = _context.TblDistrictBlots.Where(e => e.PatientId == id).ToList(),
                TblIncomingBloods = _context.TblIncomingBloods.Where(e => e.PatientId == id).ToList(),
                ListSendDistricts = _context.ListSendDistricts.ToList(),
                ListCategories = _context.ListCategories.ToList(),
                ListQualitySerums = _context.ListQualitySerums.ToList()
            };
            ViewBag.Title = "InputIncBlood";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> Update(InputRegPatient list)
        {
            List<string> ErrList = new();
            int? sexId = null, regionId = null;

            try { sexId = _context.ListSexes.First(e => e.SexNameLong == list.SexName).SexId; } 
            catch { sexId = _context.ListSexes.First(e => e.SexNameShort == list.SexName).SexId; }

            try { regionId = _context.ListRegions.First(e => e.RegionName == list.RegionName).RegionId; }
            catch { ErrList.Add($"Ошибка в регионе:{list.RegionName}"); }

            TblPatientCard tblPatientCard = new()
            {
                PatientId = list.PatientId,
                FamilyName = list.FamilyName,
                FirstName = list.FirstName,
                ThirdName = list.ThirdName,
                BirthDate = DateOnly.Parse(list.BirthDate),
                SexId = sexId,
                RegionId = regionId,
                CityName = list.CityName,
                AreaName = list.AreaName,
                PatientCom = list.PatientCom,
                PhoneNum = list.PhoneNum,
                AddrHome = list.AddrHome,
                AddrCorps = list.AddrCorps,
                AddrFlat = list.AddrFlat,
                AddrStreat = list.AddrStreat,
                Snils = list.Snils
            };
            if (ErrList.Count != 0)
                return RedirectToAction("Index", "Error", new { list = ErrList });

            _context.TblPatientCards.Update(tblPatientCard);
            _context.SaveChanges();
            return RedirectToAction("Index", "RegIncBloods", new { id = tblPatientCard.PatientId });
        }
    }
}
