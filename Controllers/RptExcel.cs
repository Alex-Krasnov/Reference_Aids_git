using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Common;
using HIVBackend.Helpers;
using System.Data;

namespace Reference_Aids.Controllers
{
    public class RptExcel : Controller
    {
        private readonly Reference_AIDSContext _context;

        public RptExcel(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create(string dat1, string dat2)
        {
            string file_type = "text/plain",
            file_name = "report.xlsx";

            DateOnly date_start = DateOnly.Parse(dat1);
            DateOnly date_end = DateOnly.Parse(dat2);

            List<string> columName = new()
            {
                "Рег. номер",
                "ФИО",
                "Дата рождения, пол",
                "Место жительства",
                "Код",
                "ЛПУ (кем направлен)",
                "Направившая лаборатория",
                "Результат",
            };

            var createExcel = new ExcelCreator();
            string fileName = $"report_{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}.xlsx";
            string path = Path.Combine(Environment.CurrentDirectory, fileName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            var lisForInput = (from patient in _context.TblPatientCards
                               join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                               select new
                               {
                                   patient.PatientId,
                                   patient.FamilyName,
                                   patient.FirstName,
                                   patient.ThirdName,
                                   patient.BirthDate,
                                   patient.PhoneNum,
                                   patient.Sex,
                                   patient.Region,
                                   patient.CityName,
                                   patient.AreaName,
                                   patient.AddrStreat,
                                   patient.AddrHome,
                                   patient.AddrCorps,
                                   patient.AddrFlat,
                                   incBlood.CategoryPatientId,
                                   incBlood.SendLabNavigation,
                                   incBlood.SendDistrictNavigation,
                                   incBlood.NumIfa,
                                   incBlood.DateBloodImport,
                                   incBlood.BloodId,
                                   incBlood.Repeat,
                                   incBlood.SendDistrictId,
                                   patient.RegionId,
                                   incBlood.SendLabId,
                                   patient.SexId
                               }).Where(e => (e.DateBloodImport.CompareTo(date_start) >= 0 && e.DateBloodImport.CompareTo(date_end) <= 0)).OrderBy(e => e.NumIfa).ToList();




            var blots = _context.TblDistrictBlots
                .Where(e => e.DBlot >= DateOnly.Parse(dat1) && e.DBlot <= DateOnly.Parse(dat2))
                .GroupBy(e => e.PatientId)
                .Select(g => g.OrderByDescending(e => e.DBlot)
                    .ThenByDescending(e => e.PatientId)
                    .First())
                .ToList();

            List<IDictionary<string, object>> lst = new();

            foreach (var item in lisForInput)
            {
                Dictionary<string, object> row = new();

                row["Рег. номер"] = item.NumIfa;

                row["ФИО"] = $"{item.FamilyName} {item.FirstName} {item.ThirdName}";

                row["Дата рождения, пол"] = $"{item.BirthDate.ToString("dd.MM.yyyy")} { _context.ListSexes.FirstOrDefault(e => e.SexId == item.SexId)?.SexNameShort}";

                row["Место жительства"] = 
                    $"{_context.ListRegions.FirstOrDefault(e => e.RegionId == item.RegionId)?.RegionName} " +
                    $"{item.CityName} {item.AreaName} {item.AddrStreat} {item.AddrHome} {item.AddrCorps} {item.AddrFlat}";

                row["Код"] = item.CategoryPatientId?.ToString();

                row["ЛПУ (кем направлен)"] = _context.ListSendDistricts.FirstOrDefault(e => e.SendDistrictId == item.SendDistrictId)?.SendDistrictName;

                row["Направившая лаборатория"] = _context.ListSendLabs.FirstOrDefault(e => e.SendLabId == item.SendLabId)?.SendLabName;

                string strAnalyzes = "";
                var resIfa = _context.TblResultIfas.Where(e => e.BloodId == item.BloodId).ToList();
                var resPcr = _context.TblResultPcrs.Where(e => e.BloodId == item.BloodId).ToList();
                var resAntigen = _context.TblResultAntigens.Where(e => e.BloodId == item.BloodId).ToList();
                var resBlot = _context.TblResultBlots.Where(e => e.BloodId == item.BloodId).ToList();

                foreach (var ifa in resIfa)
                {
                    strAnalyzes += $"ИФА {ifa.ResultIfaDate:dd-MM-yyyy}, {_context.ListResults.First(e => e.ResultId == ifa.ResultIfaResultId).ResultNameForRpt}; ";
                }

                foreach (var blot in resBlot)
                {
                    strAnalyzes += $"ИБ {blot.ResultBlotDate:dd-MM-yyyy}, {_context.ListResults.First(e => e.ResultId == blot.ResultBlotResultId).ResultNameForRpt}; ";
                }

                foreach (var pcr in resPcr)
                {
                    strAnalyzes += $"ПЦР {pcr.ResultPcrDate:dd-MM-yyyy}, {_context.ListResults.First(e => e.ResultId == pcr.ResultPcrResultId).ResultNameForRpt}, {pcr.IntResultPcr} коп/мл;";
                }

                row["Результат"] = strAnalyzes;

                lst.Add(row);
            }

            createExcel.CreateExcel(lst, path, columName);


            return PhysicalFile(path, file_type, file_name);
        }
    }
}

