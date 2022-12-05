using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;
using Reference_Aids.ModelsForInput;
using Reference_Aids.ViewModels;
using System.Globalization;

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
                ListCategories = await _context.ListCategories.OrderBy(e => e.CategoryId).ToListAsync(),
                ListQualitySerums = await _context.ListQualitySerums.ToListAsync()
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
                await _context.SaveChangesAsync();
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
                    CategoryPatientId = list.CategoryPatient,   //CategoryPatientId(_context),
                    AnonymousPatient = list.AnonymousPatient,
                    DateBloodSampling = DateOnly.Parse(list.DateBloodSampling),
                    QualitySerumId = _context.ListQualitySerums.First(e => e.QualitySerumName == list.QualitySerum).QualitySerumId,
                    DateBloodImport = DateOnly.Parse(list.DateBloodImport),
                    NumIfa = list.NumIfa,
                    NumInList = list.NumInList
                };
                _context.TblIncomingBloods.Add(tblIncomingBlood);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = list.PatientId });
            }
            List<string> ErrList = new() { "Ошибка в данных по пробирке" };
            return RedirectToAction("Index", "Error", new { list = ErrList });
        }
    }
}
