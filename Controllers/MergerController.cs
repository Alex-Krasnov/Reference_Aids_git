using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class MergerController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public MergerController(Reference_AIDSContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Title = "MergerPatient";
            return View("Index");
        }

        
        public async Task<IActionResult> Preview(int PatientId1, int PatientId2)
        {
            var patients = new List<PatientFull>
            {
                new PatientFull()
                {
                    TblPatientCards = await _context.TblPatientCards.Where(e => e.PatientId == PatientId1).ToListAsync(),
                    TblDistrictBlots = await _context.TblDistrictBlots.Where(e => e.PatientId == PatientId1).ToListAsync(),
                    TblIncomingBloods = await _context.TblIncomingBloods.Where(e => e.PatientId == PatientId1).ToListAsync()
                },
                new PatientFull()
                {
                    TblPatientCards = await _context.TblPatientCards.Where(e => e.PatientId == PatientId2).ToListAsync(),
                    TblDistrictBlots = await _context.TblDistrictBlots.Where(e => e.PatientId == PatientId2).ToListAsync(),
                    TblIncomingBloods = await _context.TblIncomingBloods.Where(e => e.PatientId == PatientId2).ToListAsync()
                }
            };

            var Viewdata = new ListForMergerViewModel
            {
                ListSexes = await _context.ListSexes.ToListAsync(),
                ListRegions = await _context.ListRegions.ToListAsync(),
                ListSendLabs = await _context.ListSendLabs.ToListAsync(),
                ListTestSystems = await _context.ListTestSystems.ToListAsync(),
                Patients = patients,
                ListSendDistricts = await _context.ListSendDistricts.ToListAsync(),
                ListCategories = await _context.ListCategories.OrderBy(e => e.CategoryId).ToListAsync(),
                ListQualitySerums = await _context.ListQualitySerums.ToListAsync()
            };
            ViewBag.Title = "MergerPatientPreview";
            return View("Preview", Viewdata);
        }

        [HttpGet]
        public async Task<IActionResult> Merger(int PatientId_1, int PatientId_2)
        {
            List<TblDistrictBlot> districtBlots = _context.TblDistrictBlots.Where(e => e.PatientId == PatientId_2).ToList();
            if (districtBlots != null)
            {
                foreach (var b in districtBlots)
                {
                    b.PatientId = PatientId_1;
                    await _context.SaveChangesAsync();
                }
            }

            List<TblIncomingBlood> incomingBloods = _context.TblIncomingBloods.Where(e => e.PatientId == PatientId_2).ToList();
            if (incomingBloods != null)
            {
                foreach (var i in incomingBloods)
                {
                    i.PatientId = PatientId_1;
                    await _context.SaveChangesAsync();
                }
            }

            var patient = _context.TblPatientCards.First(e => e.PatientId == PatientId_2);
            _context.TblPatientCards.Remove(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
