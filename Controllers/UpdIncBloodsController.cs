using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;
using Reference_Aids.ModelsForInput;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class UpdIncBloodsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public UpdIncBloodsController(Reference_AIDSContext context)
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
                TblDistrictBlots = await _context.TblDistrictBlots.Where(e => e.PatientId == id).OrderBy(e => e.DBlot).ToListAsync(),
                TblIncomingBloods = await _context.TblIncomingBloods.Where(e => e.PatientId == id).OrderBy(e => e.NumIfa).ToListAsync(),
                ListSendDistricts = await _context.ListSendDistricts.ToListAsync(),
                ListCategories = await _context.ListCategories.OrderBy(e => e.CategoryId).ToListAsync(),
                ListQualitySerums = await _context.ListQualitySerums.ToListAsync()
            };
            ViewBag.Title = "InputIncBlood";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> UpdDistrictBlot(UpdDistricBlot list)  
        {
            List<string> ErrList = new();
            int? testSystem = null, sendDistrict = null, sendLab = null;

            try { testSystem = _context.ListTestSystems.First(e => e.TestSystemName == list.TestSystemId).TestSystemId; } catch { ErrList.Add($"Ошибка тест системы:{list.TestSystemId}"); }
            try { sendDistrict = _context.ListSendDistricts.First(e => e.SendDistrictName == list.SendDistrictId).SendDistrictId; } catch { ErrList.Add($"Ошибка кем напр.:{list.SendDistrictId}"); }
            try { sendLab = _context.ListSendLabs.First(e => e.SendLabName == list.SendLabId).SendLabId; } catch { ErrList.Add($"Ошибка отпр. лаб.:{list.SendLabId}"); }

            TblDistrictBlot tblDistrictBlot = new()
            {
                PatientId = list.PatientId,
                DistrictBlotId = list.DistrictBlotId,
                DBlot = DateOnly.Parse(list.DBlot),
                CutOff = list.CutOff,
                BlotResult = list.BlotResult,
                BlotCoefficient = list.BlotCoefficient,
                TestSystemId = testSystem,
                SendDistrictId = sendDistrict,
                SendLabId = sendLab
            };

            if (ErrList.Count != 0)
                return RedirectToAction("Index", "Error", new { list = ErrList });

            _context.TblDistrictBlots.Update(tblDistrictBlot);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = list.PatientId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdIncomingBlot(UpdIncomingBlood list)
        {
            List<string> ErrList = new();
            int? qualitySerumId = null, sendDistrict = null, sendLab = null;
            DateOnly dateBloodSampling, dateBloodImport;
            bool repeat = false;

            if(list.Repeat == "on")
                repeat = true;


            try { qualitySerumId = _context.ListQualitySerums.FirstOrDefault(e => e.QualitySerumName == list.QualitySerumId)?.QualitySerumId; } 
            catch { ErrList.Add($"Ошибка тест системы:{list.QualitySerumId}"); }

            try { sendDistrict = _context.ListSendDistricts.FirstOrDefault(e => e.SendDistrictName == list.SendDistrictId)?.SendDistrictId; } 
            catch { ErrList.Add($"Ошибка кем напр.:{list.SendDistrictId}"); }

            try { sendLab = _context.ListSendLabs.FirstOrDefault(e => e.SendLabName == list.SendLabId)?.SendLabId; } 
            catch { ErrList.Add($"Ошибка отпр. лаб.:{list.SendLabId}"); }

            try { dateBloodSampling = DateOnly.Parse(list.DateBloodSampling); }
            catch { ErrList.Add($"Ошибка даты забора крови:{list.DateBloodSampling}"); }

            try { dateBloodImport = DateOnly.Parse(list.DateBloodImport); }
            catch { ErrList.Add($"Ошибка даты ввода:{list.DateBloodImport}"); }

            TblIncomingBlood tblIncomingBlood = new()
            {
                BloodId = list.BloodId,
                PatientId = list.PatientId,
                SendDistrictId = sendDistrict,
                SendLabId = sendLab,
                CategoryPatientId = list.CategoryPatientId,
                DateBloodSampling = dateBloodSampling,
                QualitySerumId = qualitySerumId,
                DateBloodImport = dateBloodImport,
                NumIfa = list.NumIfa,
                NumInList = list.NumInList,
                Repeat = repeat
            };

            if (ErrList.Count != 0)
                return RedirectToAction("Index", "Error", new { list = ErrList });

            _context.TblIncomingBloods.Update(tblIncomingBlood);
            _context.SaveChanges();
            return RedirectToAction("Index", new { id = list.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> DelDistrictBlot(int id, int patientid)
        {
            var a = _context.TblDistrictBlots.First(e => e.DistrictBlotId == id);
            _context.TblDistrictBlots.Remove(a);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = patientid });
        }

        [HttpGet]
        public async Task<IActionResult> DelIncBlood(int id, int patientid)
        {
            var a = _context.TblIncomingBloods.First(e => e.BloodId == id);
            _context.TblIncomingBloods.Remove(a);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = patientid });
        }
    }
}
