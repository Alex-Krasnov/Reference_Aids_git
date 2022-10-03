using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Reference_Aids.Models;
using Microsoft.EntityFrameworkCore;

namespace Reference_Aids.Controllers
{
    public class RptNoticeController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public RptNoticeController(Reference_AIDSContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult Create(string dat1, string dat2)
        {
            string path_from = @$"C:\work\Reference_Aids\Files\Output\ReportNotice_{DateTime.Now:dd_MM_yyyy}.docx",
            file_type = "text/plain",
            file_name = "ReportNotice.docx";

            DateOnly date_start = DateOnly.Parse(dat1);
            DateOnly date_end = DateOnly.Parse(dat2);


            var a = (from patient in _context.TblPatientCards
                     join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                     from resIfa in _context.TblResultIfas.Where(resIfa => resIfa.BloodId == incBlood.BloodId).DefaultIfEmpty()
                     from resPcr in _context.TblResultPcrs.Where(resPcr => resPcr.BloodId == incBlood.BloodId).DefaultIfEmpty() 
                     from resIb in _context.TblResultBlots.Where(resIb => resIb.BloodId == incBlood.BloodId).DefaultIfEmpty()
                     select new
                     {
                         patient.PatientId,
                         patient.FamilyName,
                         patient.FirstName,
                         patient.ThirdName,
                         patient.BirthDate,
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
                         resIb.ResultBlotDate,
                         resIb.ResultBlotResult,
                         resIb.ResultBlotResultId,
                         resIfa.ResultIfaDate,
                         resIfa.ResultIfaResult,
                         resIfa.ResultIfaResultId,
                         resPcr.ResultPcrDate,
                         resPcr.ResultPcrResult,
                         resPcr.ResultPcrResultId
                     }).Where(e => (e.ResultBlotDate.CompareTo(date_start) >= 0 && e.ResultBlotDate.CompareTo(date_end) <= 0 && e.ResultBlotResultId == 0) ||
                                   (e.ResultIfaDate.CompareTo(date_start) >= 0 && e.ResultIfaDate.CompareTo(date_end) <= 0 && e.ResultIfaResultId == 0) ||
                                   (e.ResultPcrDate.CompareTo(date_start) >= 0 && e.ResultPcrDate.CompareTo(date_end) <= 0 && e.ResultPcrResultId == 0)).ToList();

            FileInfo fileInf1 = new(path_from);
            if (fileInf1.Exists)
                fileInf1.Delete();

            CreateFile(path_from, _context);
            
            var lisForInput = (from patient in _context.TblPatientCards
                               join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                               join resIfa in _context.TblResultIfas on incBlood.BloodId equals resIfa.BloodId //переделать на left join 
                               join resPcr in _context.TblResultPcrs on incBlood.BloodId equals resPcr.BloodId //
                               join resIb in _context.TblResultBlots on incBlood.BloodId equals resIb.BloodId  //
                               select new
                               {
                                   patient.PatientId,
                                   patient.FamilyName,
                                   patient.FirstName,
                                   patient.ThirdName,
                                   patient.BirthDate,
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
                                   resIb.ResultBlotDate,
                                   resIb.ResultBlotResult,
                                   resIb.ResultBlotResultId,
                                   resIfa.ResultIfaDate,
                                   resIfa.ResultIfaResult,
                                   resIfa.ResultIfaResultId,
                                   resPcr.ResultPcrDate,
                                   resPcr.ResultPcrResult,
                                   resPcr.ResultPcrResultId
                               }).Where(e => (e.ResultBlotDate.CompareTo(date_start) >= 0 && e.ResultBlotDate.CompareTo(date_end) <= 0 && e.ResultBlotResultId == 0) ||
                                             (e.ResultIfaDate.CompareTo(date_start) >= 0 && e.ResultIfaDate.CompareTo(date_end) <= 0 && e.ResultIfaResultId == 0) ||
                                             (e.ResultPcrDate.CompareTo(date_start) >= 0 && e.ResultPcrDate.CompareTo(date_end) <= 0 && e.ResultPcrResultId == 0)).ToList();
            int i = 0;
            foreach (var item in lisForInput)
            {
                string fio = "", dateSex = "", residence = "", categery = "", lpuLab = "", strAnalyzes = "";

                try { fio += item.FamilyName; } 
                catch { }
                try { fio += " " + item.FirstName; }
                catch { }
                try { fio += " " + item.ThirdName; }
                catch { }

                try { dateSex += item.BirthDate.ToString("dd-MM-yyyy"); }
                catch { }
                try { dateSex += " ," + item.Sex.SexNameShort; }
                catch { }

                try { residence += item.Region.RegionName; }
                catch { }
                try { residence += " " + item.CityName; }
                catch { }
                try { residence += " " + item.AreaName; }
                catch { }
                try { residence += " " + item.AddrHome; }
                catch { }
                try { residence += " " + item.AddrCorps; }
                catch { }
                try { residence += " " + item.AddrFlat; }
                catch { }

                try { categery += item.CategoryPatientId.ToString(); }
                catch { }

                try { lpuLab += item.SendDistrictNavigation.SendDistrictName; }
                catch { }
                try { lpuLab += ", " + item.SendLabNavigation.SendLabName; }
                catch { }

                try { strAnalyzes += $"{item.ResultIfaDate:dd-MM-yyyy}, ИФА {item.ResultIfaResult.ResultNameForRpt}; "; }
                catch { }
                try { strAnalyzes += $"{item.ResultBlotDate:dd-MM-yyyy}, ИБ {item.ResultBlotResult.ResultNameForRpt}; "; }
                catch { }
                try { strAnalyzes += $"{item.ResultPcrDate:dd-MM-yyyy}, ПЦР {item.ResultPcrResult.ResultNameForRpt}; "; }
                catch { }

                EditFile(path_from, i, fio, dateSex, residence, categery, lpuLab, item.NumIfa, strAnalyzes);
                i++;
            }

            return PhysicalFile(path_from, file_type, file_name);
        }
        public static void CreateFile(string filepath, Reference_AIDSContext _context)
        {
            string h1_1 = "МОЦ СПИД, клиннико-диагностическая лаборатория",
                   h1_2 = "г.Москва, ул.Щепкина 61/2, к.8",
                   h2_1 = "Форма № 266/У-88",
                   h2_2 = "Утверждена МЗ СССР 05.08.88 №690",
                   h3 = $"ОПЕРАТИВНОЕ ДОНЕСЕНИЕ № {_context.ListNumForRptNotices.OrderBy(e => e.Num).Last().Num + 1} " +
                        $"о лицах, в крови которых обнаружены маркеры ВИЧ" +
                        $" в Эпидимиологиеский отдел Московского областного центра СПИД от референс подразделения КДЛ МОЦ СИПД";

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para1 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para1.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("14") }),
                                          new Text(h1_1 + "                                                                                                                                                               " + h2_1) { Space = SpaceProcessingModeValues.Preserve }));

                Paragraph para2 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para2.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("14") }),
                                          new Text(h1_2 + "                                                                                                                                                                       " + h2_2) { Space = SpaceProcessingModeValues.Preserve }));

                Paragraph para3 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center })));
                para3.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") }),
                                          new Text() { Space = SpaceProcessingModeValues.Preserve }));
                Paragraph para4 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center })));
                para4.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") }),
                                          new Text(h3) { Space = SpaceProcessingModeValues.Preserve }));
                Paragraph para5 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center })));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") }),
                                          new Text()));

                Table table = new(new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 })));

                TableRow tr = new();

                TableCell tc1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "500" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("№"))));
                tr.Append(tc1);
                TableCell tc2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2500" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("ФИО"))));
                tr.Append(tc2);
                TableCell tc3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "500" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Дата рождения, пол"))));
                tr.Append(tc3);
                TableCell tc4 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1500" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Место жительства"))));
                tr.Append(tc4);
                TableCell tc5 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "800" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Код"))));
                tr.Append(tc5);
                TableCell tc6 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1000" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("ЛПУ, лаборатория"))));
                tr.Append(tc6);
                TableCell tc7 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "800" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Рег. номер"))));
                tr.Append(tc7);
                TableCell tc8 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2500" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Дата постановки, Результат"))));
                tr.Append(tc8);
                TableCell tc10 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "950" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Примечение"))));
                tr.Append(tc10);
                table.Append(tr);
                body.Append(table);
            }
            //_context.ListNumForRptNotices.Add(new ListNumForRptNotice { Num = _context.ListNumForRptNotices.Last().Num + 1 });
            //_context.SaveChanges();
        }

        public static void EditFile(string filepath, int i, string fio, string dateSex, string residence, string categery, string lpuLab, int numIfa, string strAnalyzes)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;
                Table table = body.Elements<Table>().First();
                TableRow tr2 = new();

                TableCell tc1_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(i.ToString()))));
                tr2.Append(tc1_2);
                TableCell tc2_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(fio) { Space = SpaceProcessingModeValues.Preserve })));
                tr2.Append(tc2_2);
                TableCell tc3_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(dateSex) { Space = SpaceProcessingModeValues.Preserve })));
                tr2.Append(tc3_2);
                TableCell tc4_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(residence) { Space = SpaceProcessingModeValues.Preserve })));
                tr2.Append(tc4_2);
                TableCell tc5_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(categery))));
                tr2.Append(tc5_2);
                TableCell tc6_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(lpuLab))));
                tr2.Append(tc6_2);
                TableCell tc7_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(numIfa.ToString()))));
                tr2.Append(tc7_2);
                TableCell tc8_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(strAnalyzes))));
                tr2.Append(tc8_2);
                TableCell tc10_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text())));
                tr2.Append(tc10_2);
                table.Append(tr2);
            }
        }
    }
}
