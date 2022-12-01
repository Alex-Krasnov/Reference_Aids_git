using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.ModelsForInput;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class SearchPatientsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public SearchPatientsController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ListForSearchPatientViewModel viewModel = new()
            {
                ListRegions = await _context.ListRegions.ToListAsync(),
                ListSexes = await _context.ListSexes.ToListAsync()
            };
            ViewBag.Title = "SearchPatient";
            return View("Index", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Search(InputSearch list)
        {
            if (ModelState.IsValid)
            {
                string sql = "select tbl_patient_card.* from tbl_patient_card left join tbl_incoming_blood on tbl_patient_card.patient_id = tbl_incoming_blood.patient_id where 1=1";

                if (list.PatientId != null)
                    sql += $" and tbl_patient_card.patient_id = {list.PatientId}";

                if (list.BirthDate != null)
                    sql += $" and birth_date = '{list.BirthDate}'";

                if (list.SexId(_context) != null)
                    sql += $" and sex_id = {list.SexId(_context)}";

                if (list.RegionId(_context) != null)
                    sql += $" and region_id = {list.RegionId(_context)}";

                if (list.FirstName != null)
                    sql += $" and first_name ilike '{list.FirstName}%'";

                if (list.ThirdName != null)
                    sql += $" and third_name ilike '{list.ThirdName}%'";

                if (list.FamilyName != null)
                    sql += $" and family_name ilike '{list.FamilyName}%'";

                if (list.NumIfa != null)
                    sql += $" and tbl_incoming_blood.num_ifa = {list.NumIfa}";

                sql += " group by tbl_patient_card.patient_id, first_name, family_name, third_name, birth_date, sex_id, region_id, patient_com, phone_num, d_edit, user_edit, addr_home, addr_corps, addr_flat, addr_streat, city_name, area_name, die_id, old_nom order by first_name, family_name, third_name, birth_date";
                ListForSearchPatientViewModel viewModel = new()
                {
                    ListRegions = await _context.ListRegions.ToListAsync(),
                    ListSexes = await _context.ListSexes.ToListAsync(),
                    ID = list.PatientId,
                    FamilyName = list.FamilyName,
                    FirstName = list.FirstName,
                    ThirdName = list.ThirdName,
                    BirthDate = list.BirthDate,
                    SexName = list.SexName,
                    RegoinName = list.RegionName,
                    NumIfa = list.NumIfa,
                    TblPatientCards = await _context.TblPatientCards.FromSqlRaw(sql).ToListAsync()
                };
                return View("Search", viewModel);
            }
            return RedirectToAction("Index");
        }
    }
}
