﻿using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Reference_Aids.ModelsForInput;
using Reference_Aids.Data;
using Microsoft.EntityFrameworkCore;

namespace Reference_Aids.Controllers
{
    public class FilesController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public FilesController(Reference_AIDSContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string path_to = @$"C:\work\Reference_Aids\Files\Output\form4_{DateTime.Now:dd_MM_yyyy}.xlsx";
            string path_from = @"C:\work\Reference_Aids\Files\Sample\form4.xlsx";
            string file_type = "text/plain";
            var file_name = "form4.xlsx";
            string dat1 = "1900-01-01", dat2 = "2022-09-19";

            FileInfo fileInf1 = new(path_to);
            if (fileInf1.Exists)
                fileInf1.Delete();

            FileInfo fileInf = new(path_from);
            fileInf.CopyTo(path_to);

            int[][] listCat = ListforForm4(dat1, dat2);

            int i = 0, j = 2;

            foreach(var cat in listCat)
            {
                foreach(char c in "DEFGHIJKLMNOP")
                {
                    ChangeTextInCell(path_to, cat[j], cat[1], c);
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

        public int[][] ListforForm4(string date1, string date2)
        {
            var cat = _context.ListCategories.Select(e => e.CategoryId).ToList();
            //List<Form4> form4 = new();
            DateOnly old_18 = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
                    old_14 = DateOnly.FromDateTime(DateTime.Today.AddYears(-14)),
                    date_start = DateOnly.Parse(date1),
                    date_end = DateOnly.Parse(date2);

            int i = 0;
            int[][] form = new int[19][];

            foreach (int item in cat)
            {
                int total_4 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport })
                                                      .Where(e => e.CategoryPatientId == item
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                      .Count();
                int man_5 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport })
                                                    .Where(e => e.SexId == 0
                                                            && e.CategoryPatientId == item
                                                            && e.BirthDate.CompareTo(old_18) <= 0
                                                            && e.DateBloodImport.CompareTo(date_start) >= 0
                                                            && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                    .Count();
                int woman_6 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport })
                                                      .Where(e => e.SexId == 1
                                                              && e.CategoryPatientId == item
                                                              && e.BirthDate.CompareTo(old_18) <= 0
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                      .Count();
                int child_7 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport })
                                                      .Where(e => e.CategoryPatientId == item
                                                              && e.BirthDate.CompareTo(old_14) <= 0
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                      .Count();
                int teenager_8 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.SexId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport })
                                                      .Where(e => e.CategoryPatientId == item
                                                              && e.BirthDate.CompareTo(old_14) > 0
                                                              && e.BirthDate.CompareTo(old_18) < 0
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                      .Count();
                int anon_9 = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.PatientId, p.BirthDate, i.CategoryPatientId, i.DateBloodImport, i.AnonymousPatient })
                                                      .Where(e => e.CategoryPatientId == item
                                                              && e.AnonymousPatient == true
                                                              && e.DateBloodImport.CompareTo(date_start) >= 0
                                                              && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                      .Count();



                int ifa = (from patient in _context.TblPatientCards
                           join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                           join resIfa in _context.TblResultIfas on incBlood.BloodId equals resIfa.BloodId
                           select new
                           {
                               incBlood.CategoryPatientId,
                               patient.BirthDate,
                               incBlood.DateBloodImport,
                               resIfa.ResultIfaResultId
                           })
                           .Where(e => e.CategoryPatientId == item
                                    && e.DateBloodImport.CompareTo(date_start) >= 0
                                    && e.DateBloodImport.CompareTo(date_end) <= 0
                                    && e.ResultIfaResultId == 0)
                           .Count();

                int totalPcr = _context.TblIncomingBloods.Join(_context.TblResultPcrs, p => p.BloodId, i => i.BloodId,
                                                                (p, i) => new {p.CategoryPatientId, p.DateBloodImport})
                                                         .Where(e => e.CategoryPatientId == item
                                                                && e.DateBloodImport.CompareTo(date_start) >= 0
                                                                && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                         .Count();
                int totalIb = _context.TblIncomingBloods.Join(_context.TblResultBlots, p => p.BloodId, i => i.BloodId,
                                                                (p, i) => new { p.CategoryPatientId, p.DateBloodImport })
                                                         .Where(e => e.CategoryPatientId == item
                                                                && e.DateBloodImport.CompareTo(date_start) >= 0
                                                                && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                         .Count();
                int totalAntigen = _context.TblIncomingBloods.Join(_context.TblResultAntigens, p => p.BloodId, i => i.BloodId,
                                                                (p, i) => new { p.CategoryPatientId, p.DateBloodImport })
                                                         .Where(e => e.CategoryPatientId == item
                                                                && e.DateBloodImport.CompareTo(date_start) >= 0
                                                                && e.DateBloodImport.CompareTo(date_end) <= 0)
                                                         .Count();
                int totalIfa = _context.TblIncomingBloods.Join(_context.TblResultIfas, p => p.BloodId, i => i.BloodId,
                                                                (p, i) => new { p.CategoryPatientId, p.DateBloodImport })
                                                         .Where(e => e.CategoryPatientId == item
                                                                && e.DateBloodImport.CompareTo(date_start) >= 0
                                                                && e.DateBloodImport.CompareTo(date_end) <= 0)
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
                                      resPcr.ResultPcrResultId
                                  })
                                  .Where(e => e.SexId == 0
                                         && e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_18) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultPcrResultId == 0)
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
                                      resIb.ResultBlotResultId
                                  })
                                  .Where(e => e.SexId == 0
                                         && e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_18) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultBlotResultId == 0)
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
                                      resPcr.ResultPcrResultId
                                    })
                                  .Where(e => e.SexId == 1
                                         && e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_18) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultPcrResultId == 0)
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
                                     resIb.ResultBlotResultId
                                 })
                                  .Where(e => e.SexId == 1
                                         && e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_18) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultBlotResultId == 0)
                                  .Count(); 

                int child_15_pcr = (from patient in _context.TblPatientCards
                                    join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                    join resPcr in _context.TblResultPcrs on incBlood.BloodId equals resPcr.BloodId
                                    select new
                                    {
                                        incBlood.CategoryPatientId,
                                        patient.BirthDate,
                                        incBlood.DateBloodImport,
                                        resPcr.ResultPcrResultId
                                    })
                                  .Where(e => e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_14) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultPcrResultId == 0)
                                  .Count(); 

                int child_15_ib = (from patient in _context.TblPatientCards
                                   join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                   join resIb in _context.TblResultBlots on incBlood.BloodId equals resIb.BloodId
                                   select new
                                   {
                                       incBlood.CategoryPatientId,
                                       patient.BirthDate,
                                       incBlood.DateBloodImport,
                                       resIb.ResultBlotResultId
                                   })
                                  .Where(e => e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_14) <= 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultBlotResultId == 0)
                                  .Count(); 

                int teenager_16_pcr = (from patient in _context.TblPatientCards
                                       join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                       join resPcr in _context.TblResultPcrs on incBlood.BloodId equals resPcr.BloodId
                                       select new
                                    {
                                        incBlood.CategoryPatientId,
                                        patient.BirthDate,
                                        incBlood.DateBloodImport,
                                        resPcr.ResultPcrResultId
                                    })
                                  .Where(e => e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_14) > 0
                                         && e.BirthDate.CompareTo(old_18) < 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultPcrResultId == 0)
                                  .Count(); 

                int teenager_16_ib = (from patient in _context.TblPatientCards
                                   join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                                   join resIb in _context.TblResultBlots on incBlood.BloodId equals resIb.BloodId
                                   select new
                                   {
                                       incBlood.CategoryPatientId,
                                       patient.BirthDate,
                                       incBlood.DateBloodImport,
                                       resIb.ResultBlotResultId
                                   })
                                  .Where(e => e.CategoryPatientId == item
                                         && e.BirthDate.CompareTo(old_14) > 0
                                         && e.BirthDate.CompareTo(old_18) < 0
                                         && e.DateBloodImport.CompareTo(date_start) >= 0
                                         && e.DateBloodImport.CompareTo(date_end) <= 0
                                         && e.ResultBlotResultId == 0)
                                  .Count(); 


                int man_13 = man_13_pcr + man_13_ib;
                int woman_14 = woman_14_pcr + woman_14_ib;
                int child_15 = child_15_pcr + child_15_ib;
                int teenager_16 = teenager_16_pcr + teenager_16_ib;

                int pcrIb = man_13 + woman_14 + child_15 + teenager_16;
                int totalAnalyzes = totalIfa + totalPcr + totalIb + totalAntigen;
                int rowNum = _context.ListCategories.Where(e => e.CategoryId == item).Select(e => e.RowNum).ToList().First();

                //Form4 itemCategory = new()
                //{
                //    Category = item,
                //    Row = rowNum,
                //    Total_4 = total_4,
                //    Man_5 = man_5,
                //    Woman_6 = woman_6,
                //    Child_7 = child_7,
                //    Teenager_8 = teenager_8,
                //    Anon_9 = anon_9,
                //    TotalAnalyzes_10 = totalAnalyzes,
                //    Ifa_11 = ifa,
                //    PcrIb_12 = pcrIb,
                //    Man_13 = man_13,
                //    Woman_14 = woman_14,
                //    Child_15 = child_15,
                //    Teenager_16 = teenager_16

                //};
                //form4.Add(itemCategory);

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
                i++;
            }
            return form;
        }
    }
}
