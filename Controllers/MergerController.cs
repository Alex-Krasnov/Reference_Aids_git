using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.Models;

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

        [HttpPost]
        public async Task<IActionResult> Merger(int PatientId1, int PatientId2)
        {
            List<TblDistrictBlot> districtBlots = _context.TblDistrictBlots.Where(e => e.PatientId == PatientId2).ToList();
            if (districtBlots != null)
            {
                foreach (var b in districtBlots)
                {
                    b.PatientId = PatientId1;
                    await _context.SaveChangesAsync();
                }
            }

            List<TblIncomingBlood> incomingBloods = _context.TblIncomingBloods.Where(e => e.PatientId == PatientId2).ToList();
            if (incomingBloods != null)
            {
                foreach (var i in incomingBloods)
                {
                    i.PatientId = PatientId1;
                    await _context.SaveChangesAsync();
                }
            }

            var patient = _context.TblPatientCards.First(e => e.PatientId == PatientId2);
            _context.TblPatientCards.Remove(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
