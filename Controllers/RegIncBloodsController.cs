using Microsoft.AspNetCore.Mvc;
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
                ListSexes =  _context.ListSexes.ToList(),
                ListRegions =  _context.ListRegions.ToList(),
                ListSendLabs =  _context.ListSendLabs.ToList(),
                ListTestSystems =  _context.ListTestSystems.ToList(),
                TblPatientCards =  _context.TblPatientCards.Where(e => e.PatientId == id).ToList(),
                TblDistrictBlots =  _context.TblDistrictBlots.Where(e => e.PatientId == id).ToList(),
                TblIncomingBloods =  _context.TblIncomingBloods.Where(e => e.PatientId == id).ToList(),
                ListSendDistricts =  _context.ListSendDistricts.ToList(),
                ListCategories =  _context.ListCategories.OrderBy(e => e.CategoryId).ToList(),
                ListQualitySerums =  _context.ListQualitySerums.ToList()
            };
            ViewBag.Title = "InputIncBlood";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDistrictBlot(InputDistricBlot list)  
        {
            if (list.DBlot != null && list.BlotResult != null &&  list.SendDistrictId != null 
                && list.CutOff != null && list.SendLabId != null && list.TestSystemId != null) //ModelState.IsValid
            {

                TblDistrictBlot tblDistrictBlot = new()
                {
                    PatientId = list.PatientId,
                    DBlot = DateOnly.Parse(list.DBlot),
                    CutOff = Convert.ToDouble(list.CutOff),
                    BlotResult = Convert.ToDouble(list.BlotResult),
                    BlotCoefficient = Math.Round((Convert.ToDouble(list.BlotResult) / Convert.ToDouble(list.CutOff)), 3),
                    TestSystemId = _context.ListTestSystems.First(e => e.TestSystemName == list.TestSystemId).TestSystemId,
                    SendDistrictId =_context.ListSendDistricts.First(e => e.SendDistrictName == list.SendDistrictId).SendDistrictId,
                    SendLabId = _context.ListSendLabs.First(e => e.SendLabName == list.SendLabId).SendLabId
                };

                _context.TblDistrictBlots.Add(tblDistrictBlot);
                _context.SaveChanges();
                List<string> ErrList = new() { "Ошибка в блоте" };

                if(!ModelState.IsValid)
                    return RedirectToAction("Index", "Error", new { list = ErrList });

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
                    SendDistrictId =  _context.ListSendDistricts.First(e => e.SendDistrictName == list.SendDistrict).SendDistrictId,
                    SendLabId = _context.ListSendLabs.First(e => e.SendLabName == list.SendLab).SendLabId,
                    CategoryPatientId = list.CategoryPatient,
                    Repeat = list.Repeat == "on",
                    DateBloodSampling = DateOnly.Parse(list.DateBloodSampling),
                    QualitySerumId = _context.ListQualitySerums.First(e => e.QualitySerumName == list.QualitySerum).QualitySerumId,
                    DateBloodImport = DateOnly.Parse(list.DateBloodImport),
                    NumIfa = list.NumIfa,
                    NumInList = list.NumInList
                };
                _context.TblIncomingBloods.Add(tblIncomingBlood);
                _context.SaveChanges();
                return RedirectToAction("Index", new { id = list.PatientId });
            }
            List<string> ErrList = new() { "Ошибка в данных по пробирке" };
            return RedirectToAction("Index", "Error", new { list = ErrList });
        }
    }
}
