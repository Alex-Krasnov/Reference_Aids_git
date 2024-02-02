using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Reference_Aids.Data;

namespace Reference_Aids.Controllers
{
    public class Form4Controller : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public Form4Controller(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        [HttpPost]
        public IActionResult Create(string dat1, string dat2)
        {
            string path_to = _appEnvironment.WebRootPath + @$"\Files\Output\form4_{DateTime.Now:dd_MM_yyyy}.xlsx",
            path_from = _appEnvironment.WebRootPath + @"\Files\Sample\form4.xlsx",
            file_type = "text/plain";
            var file_name = "form4.xlsx";

            FileInfo fileInf1 = new(path_to);
            if (fileInf1.Exists)
                fileInf1.Delete();

            FileInfo fileInf = new(path_from);
            fileInf.CopyTo(path_to);

            int[][] listCat = ListforForm4(dat1, dat2);

            ChangeStrInCell(path_to, dat1, 1, 'A');
            ChangeStrInCell(path_to, dat2, 1, 'B');
            int i = 0, j = 2;

            foreach(var cat in listCat) 
            {
                foreach(char c in "DEFGHIJKLMNOP")
                {
                    try { 
                        ChangeTextInCell(path_to, cat[j], cat[1], c); 
                    }
                    catch { }
                    j++;
                }
                j = 2;
                i++;
            }
            return PhysicalFile(path_to, file_type, file_name);
        }

        public static void ChangeTextInCell(string filepath, int value, int num_row, char num_col)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true))
            {
                IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Sheet1");
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();

                Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == num_row).First();
                Cell cell = row.Elements<Cell>().Where(c => c.CellReference.Value == $"{num_col}{num_row}").First();
                cell.CellValue = new CellValue(value);
                cell.DataType = CellValues.Number;

                worksheetPart.Worksheet.Save();
            }
        }

        public static void ChangeStrInCell(string filepath, string value, int num_row, char num_col) // костыль
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true))
            {
                IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Sheet1");
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();

                Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == num_row).First();
                Cell cell = row.Elements<Cell>().Where(c => c.CellReference.Value == $"{num_col}{num_row}").First();
                cell.CellValue = new CellValue(value);
                cell.DataType = CellValues.Number;

                worksheetPart.Worksheet.Save();
            }
        }

        public int[][] ListforForm4(string date1, string date2)
        {
            var cat = _context.ListCategories.Select(e => e.CategoryId).ToList();
            DateOnly old_18 = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
                     old_14 = DateOnly.FromDateTime(DateTime.Today.AddYears(-14)),
                     date_start = DateOnly.Parse(date1),
                     date_end = DateOnly.Parse(date2);
            int i = 0;
            int[][] form = new int[19][];

            foreach (int item in cat)
            {
                int total_4 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport, i.Repeat, i.SendLabId})
                                                      .Where(e => e.CategoryPatientId == item
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0
                                                              && e.Repeat != true
                                                              && e.SendLabId != 37 
                                                              && e.SendLabId != 75)
                                                      .Count();
                int man_5 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport, i.Repeat, i.SendLabId })
                                                    .Where(e => e.SexId == 0
                                                            && e.CategoryPatientId == item
                                                            && e.BirthDate.CompareTo(old_18) <= 0
                                                            && e.DateBloodImport.CompareTo(date_start) >= 0
                                                            && e.DateBloodImport.CompareTo(date_end) <= 0
                                                            && e.Repeat != true
                                                            && e.SendLabId != 37
                                                            && e.SendLabId != 75)
                                                    .Count();
                int woman_6 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport, i.Repeat, i.SendLabId })
                                                      .Where(e => e.SexId == 1
                                                              && e.CategoryPatientId == item
                                                              && e.BirthDate.CompareTo(old_18) <= 0
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0
                                                              && e.Repeat != true
                                                              && e.SendLabId != 37
                                                              && e.SendLabId != 75)
                                                      .Count();
                int child_7 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport, i.Repeat, i.SendLabId })
                                                      .Where(e => e.CategoryPatientId == item
                                                              && e.BirthDate.CompareTo(old_14) >= 0
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0
                                                              && e.Repeat != true
                                                              && e.SendLabId != 37
                                                              && e.SendLabId != 75)
                                                      .Count();
                int teenager_8 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport, i.Repeat, i.SendLabId })
                                                      .Where(e => e.CategoryPatientId == item
                                                              && e.BirthDate.CompareTo(old_14) > 0
                                                              && e.BirthDate.CompareTo(old_18) < 0
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0
                                                              && e.Repeat != true
                                                              && e.SendLabId != 37
                                                              && e.SendLabId != 75)
                                                      .Count();
                int anon_9 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport, i.AnonymousPatient, i.Repeat, i.SendLabId })
                                                      .Where(e => e.CategoryPatientId == item
                                                              && e.AnonymousPatient == true
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0
                                                              && e.Repeat != true
                                                              && e.SendLabId != 37
                                                              && e.SendLabId != 75)
                                                      .Count();



                int ifa = (from patient in _context.TblPatientCards
                           join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                           join resIfa in _context.TblResultIfas on incBlood.BloodId equals resIfa.BloodId
                           select new
                           {
                               incBlood.CategoryPatientId,
                               patient.BirthDate,
                               incBlood.DateBloodImport,
                               resIfa.ResultIfaResultId,
                               patient.PatientId,
                               incBlood.SendLabId
                           })
                           .Where(e => e.CategoryPatientId == item
                                    && e.DateBloodImport.CompareTo(date_start) >= 0
                                    && e.DateBloodImport.CompareTo(date_end) <= 0
                                    && e.ResultIfaResultId == 0
                                    && e.SendLabId != 37
                                    && e.SendLabId != 75)
                           .Select(e => new { e.PatientId })
                           .GroupBy(e => new { e.PatientId })
                           .Count(); //distinct patient_id

                int totalPcr = _context.TblIncomingBloods.Join(_context.TblResultPcrs, p => p.BloodId, i => i.BloodId,
                                                                (p, i) => new { p.CategoryPatientId, p.DateBloodImport, p.Repeat, p.SendLabId })
                                                         .Where(e => e.CategoryPatientId == item
                                                                && e.DateBloodImport.CompareTo(date_start) >= 0
                                                                && e.DateBloodImport.CompareTo(date_end) <= 0
                                                                && e.Repeat != true
                                                                && e.SendLabId != 37
                                                                && e.SendLabId != 75)
                                                         .Count();
                int totalIb = _context.TblIncomingBloods.Join(_context.TblResultBlots, p => p.BloodId, i => i.BloodId,
                                                                (p, i) => new { p.CategoryPatientId, p.DateBloodImport, p.Repeat, p.SendLabId })
                                                         .Where(e => e.CategoryPatientId == item
                                                                && e.DateBloodImport.CompareTo(date_start) >= 0
                                                                && e.DateBloodImport.CompareTo(date_end) <= 0
                                                                && e.Repeat != true
                                                                && e.SendLabId != 37
                                                                && e.SendLabId != 75)
                                                         .Count();
                int totalAntigen = _context.TblIncomingBloods.Join(_context.TblResultAntigens, p => p.BloodId, i => i.BloodId,
                                                                (p, i) => new { p.CategoryPatientId, p.DateBloodImport, p.SendLabId })
                                                         .Where(e => e.CategoryPatientId == item
                                                                && e.DateBloodImport.CompareTo(date_start) >= 0
                                                                && e.DateBloodImport.CompareTo(date_end) <= 0
                                                                && e.SendLabId != 37
                                                                && e.SendLabId != 75)
                                                         .Count();
                int totalIfa = _context.TblIncomingBloods.Join(_context.TblResultIfas, p => p.BloodId, i => i.BloodId,
                                                                (p, i) => new { p.CategoryPatientId, p.DateBloodImport, p.SendLabId })
                                                         .Where(e => e.CategoryPatientId == item
                                                                && e.DateBloodImport.CompareTo(date_start) >= 0
                                                                && e.DateBloodImport.CompareTo(date_end) <= 0
                                                                && e.SendLabId != 37
                                                                && e.SendLabId != 75)
                                                         .Count();


                int man_13_pcr = (from patient in _context.TblPatientCards
                                  join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                  join resPcr in _context.TblResultPcrs on incBlood.BloodId equals resPcr.BloodId
                                  select new
                                  {
                                      patient.SexId,
                                      incBlood.CategoryPatientId,
                                      patient.BirthDate,
                                      incBlood.DateBloodImport,
                                      resPcr.ResultPcrResultId,
                                      incBlood.Repeat,
                                      incBlood.SendLabId
                                  })
                                  .Where(e => e.SexId == 0
                                         && e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_18) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultPcrResultId == 0
                                         && e.Repeat != true
                                         && e.SendLabId != 37
                                         && e.SendLabId != 75)
                                  .Count();

                int man_13_ib = (from patient in _context.TblPatientCards
                                 join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                 join resIb in _context.TblResultBlots on incBlood.BloodId equals resIb.BloodId
                                 select new
                                 {
                                     patient.SexId,
                                     incBlood.CategoryPatientId,
                                     patient.BirthDate,
                                     incBlood.DateBloodImport,
                                     resIb.ResultBlotResultId,
                                     incBlood.Repeat,
                                     incBlood.SendLabId
                                 })
                                  .Where(e => e.SexId == 0
                                         && e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_18) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultBlotResultId == 0
                                         && e.Repeat != true
                                         && e.SendLabId != 37
                                         && e.SendLabId != 75)
                                  .Count();

                int woman_14_pcr = (from patient in _context.TblPatientCards
                                    join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                    join resPcr in _context.TblResultPcrs on incBlood.BloodId equals resPcr.BloodId
                                    select new
                                    {
                                        patient.SexId,
                                        incBlood.CategoryPatientId,
                                        patient.BirthDate,
                                        incBlood.DateBloodImport,
                                        resPcr.ResultPcrResultId,
                                        incBlood.Repeat,
                                        incBlood.SendLabId
                                    })
                                  .Where(e => e.SexId == 1
                                         && e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_18) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultPcrResultId == 0
                                         && e.Repeat != true
                                         && e.SendLabId != 37
                                         && e.SendLabId != 75)
                                  .Count();

                int woman_14_ib = (from patient in _context.TblPatientCards
                                   join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                   join resIb in _context.TblResultBlots on incBlood.BloodId equals resIb.BloodId
                                   select new
                                   {
                                       patient.SexId,
                                       incBlood.CategoryPatientId,
                                       patient.BirthDate,
                                       incBlood.DateBloodImport,
                                       resIb.ResultBlotResultId,
                                       incBlood.Repeat,
                                       incBlood.SendLabId
                                   })
                                  .Where(e => e.SexId == 1
                                         && e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_18) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultBlotResultId == 0
                                         && e.Repeat != true
                                         && e.SendLabId != 37
                                         && e.SendLabId != 75)
                                  .Count();

                int child_15_pcr = (from patient in _context.TblPatientCards
                                    join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                    join resPcr in _context.TblResultPcrs on incBlood.BloodId equals resPcr.BloodId
                                    select new
                                    {
                                        incBlood.CategoryPatientId,
                                        patient.BirthDate,
                                        incBlood.DateBloodImport,
                                        resPcr.ResultPcrResultId,
                                        incBlood.Repeat,
                                        incBlood.SendLabId
                                    })
                                  .Where(e => e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_14) >= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultPcrResultId == 0
                                         && e.Repeat != true
                                         && e.SendLabId != 37
                                         && e.SendLabId != 75)
                                  .Count();

                int child_15_ib = (from patient in _context.TblPatientCards
                                   join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                   join resIb in _context.TblResultBlots on incBlood.BloodId equals resIb.BloodId
                                   select new
                                   {
                                       incBlood.CategoryPatientId,
                                       patient.BirthDate,
                                       incBlood.DateBloodImport,
                                       resIb.ResultBlotResultId,
                                       incBlood.Repeat,
                                       incBlood.SendLabId
                                   })
                                  .Where(e => e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_14) >= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultBlotResultId == 0
                                         && e.Repeat != true
                                         && e.SendLabId != 37
                                         && e.SendLabId != 75)
                                  .Count();

                int teenager_16_pcr = (from patient in _context.TblPatientCards
                                       join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                       join resPcr in _context.TblResultPcrs on incBlood.BloodId equals resPcr.BloodId
                                       select new
                                       {
                                           incBlood.CategoryPatientId,
                                           patient.BirthDate,
                                           incBlood.DateBloodImport,
                                           resPcr.ResultPcrResultId,
                                           incBlood.Repeat,
                                           incBlood.SendLabId
                                       })
                                  .Where(e => e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_14) > 0
                                         && e.BirthDate.CompareTo(old_18) < 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultPcrResultId == 0
                                         && e.Repeat != true
                                         && e.SendLabId != 37
                                         && e.SendLabId != 75)
                                  .Count();

                int teenager_16_ib = (from patient in _context.TblPatientCards
                                      join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                      join resIb in _context.TblResultBlots on incBlood.BloodId equals resIb.BloodId
                                      select new
                                      {
                                          incBlood.CategoryPatientId,
                                          patient.BirthDate,
                                          incBlood.DateBloodImport,
                                          resIb.ResultBlotResultId,
                                          incBlood.Repeat,
                                          incBlood.SendLabId
                                      })
                                  .Where(e => e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_14) > 0
                                         && e.BirthDate.CompareTo(old_18) < 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultBlotResultId == 0
                                         && e.Repeat != true
                                         && e.SendLabId != 37
                                         && e.SendLabId != 75)
                                  .Count();


                int man_13 = man_13_pcr + man_13_ib;
                int woman_14 = woman_14_pcr + woman_14_ib;
                int child_15 = child_15_pcr + child_15_ib;
                int teenager_16 = teenager_16_pcr + teenager_16_ib;

                int pcrIb = man_13 + woman_14 + child_15 + teenager_16;
                int totalAnalyzes = totalIfa + totalPcr + totalIb + totalAntigen;
                if (item == 118)
                    totalAnalyzes += _context.TblAnalyzesControls.Where(e => e.ControlDate.CompareTo(date_start) >= 0 
                                                                            && e.ControlDate.CompareTo(date_end) <= 0)
                                                                 .Sum(e => e.ControlAmount);

                int rowNum = _context.ListCategories.Where(e => e.CategoryId == item).Select(e => e.RowNum).ToList().First();

                form[i] = new int[15] {
                    item,
                    rowNum,
                    total_4,
                    man_5,
                    woman_6,
                    child_7,
                    teenager_8,
                    anon_9,
                    totalAnalyzes,
                    ifa,
                    pcrIb,
                    man_13,
                    woman_14,
                    child_15,
                    teenager_16
                };
                //form[i] = new int[15] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                i++;
            }
            return form;
        }
    }
}
