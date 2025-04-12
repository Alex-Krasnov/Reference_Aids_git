using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Reflection.Metadata;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class RptNoticeEpidController : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public RptNoticeEpidController(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        [HttpPost]
        public IActionResult Create(string dat1, string dat2)
        {
            string path_from = _appEnvironment.WebRootPath +@$"\Files\Output\ReportNoticeEpid_{DateTime.Now:dd_MM_yyyy}.docx",
            file_type = "text/plain",
            file_name = "ReportNotice.docx";

            DateOnly date_start = DateOnly.Parse(dat1);
            DateOnly date_end = DateOnly.Parse(dat2);

            FileInfo fileInf1 = new(path_from);
            if (fileInf1.Exists)
                fileInf1.Delete();

            CreateFile(path_from, _context);

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
                                   incBlood.Repeat
                               }).Where(e => (e.DateBloodImport.CompareTo(date_start) >= 0 && e.DateBloodImport.CompareTo(date_end) <= 0)).OrderBy(e => e.FamilyName).ThenBy(e => e.FirstName).ThenBy(e => e.ThirdName).ToList();

            int i = 0;
            foreach (var item in lisForInput)
            {
                string fio = "", dateSex = "", residence = "", categery = "", lpuLab = "", strAnalyzes = "";

                var resIfa = _context.TblResultIfas.Where(e => e.BloodId == item.BloodId).ToList(); //  && e.ResultIfaResultId != 1 && e.ResultIfaResultId != null
                var resPcr = _context.TblResultPcrs.Where(e => e.BloodId == item.BloodId).ToList(); //  && e.ResultPcrResultId != 1 && e.ResultPcrResultId != null
                var resAntigen = _context.TblResultAntigens.Where(e => e.BloodId == item.BloodId).ToList(); //  && e.ResultAntigenResultId != 1 && e.ResultAntigenResultId != null
                var resBlot = _context.TblResultBlots.Where(e => e.BloodId == item.BloodId).ToList(); //  && e.ResultBlotResultId != null && e.ResultBlotResultId != null
                //var resBlot = _context.TblResultBlots.Where(e => e.BloodId == item.BloodId && e.ResultBlotResultId != 1 && e.ResultBlotResultId != null).ToList();

                if (resIfa.Count() == 0 && resPcr.Count() == 0 && resAntigen.Count() == 0 && resBlot.Count() == 0)
                    continue;

                if (!resIfa.Any(e => e.ResultIfaResultId == 0 || e.ResultIfaResultId == 2)
                    && !resPcr.Any(e => e.ResultPcrResultId == 0 || e.ResultPcrResultId == 2)
                    && !resAntigen.Any(e => e.ResultAntigenResultId == 0 || e.ResultAntigenResultId == 2)
                    && !resBlot.Any(e => e.ResultBlotResultId == 0 || e.ResultBlotResultId == 2))
                    continue;

                string strRepeat = "Первичный";

                if(item.Repeat == true)
                    strRepeat = "Повторный";

                try { fio += item.FamilyName; } 
                catch { }
                try { fio += " " + item.FirstName; }
                catch { }
                try { fio += " " + item.ThirdName + $" {strRepeat}"; }
                catch { }

                try { dateSex += item.BirthDate.ToString("dd-MM-yyyy"); }
                catch { }
                try { dateSex += " ," + item.Sex.SexNameShort; }
                catch { }

                try { residence += item.Region.RegionName; }
                catch { }
                try { residence += " г. " + item.CityName; }
                catch { }
                try { residence += " " + item.AreaName; }
                catch { }
                try { residence += "ул. " + item.AddrStreat; }
                catch { }
                try { residence += "д. " + item.AddrHome; }
                catch { }
                try { residence += "к. " + item.AddrCorps; }
                catch { }
                try { residence += "кв. " + item.AddrFlat; }
                catch { }
                try { residence += "тел. " + item.PhoneNum; }
                catch { }

                try { categery += item.CategoryPatientId.ToString(); }
                catch { }

                try { lpuLab += item.SendDistrictNavigation.SendDistrictName; }
                catch { }
                try { lpuLab += ", " + item.SendLabNavigation.SendLabName; }
                catch { }

                foreach(var ifa in resIfa)
                {
                    try { strAnalyzes += $"ИФА {ifa.ResultIfaDate:dd-MM-yyyy}, "; } catch { }
                    try { strAnalyzes += $"{_context.ListResults.First(e => e.ResultId == ifa.ResultIfaResultId).ResultNameForRpt}; "; } catch { }
                }
                    

                foreach (var blot in resBlot)
                {
                    try { strAnalyzes += $"ИБ {blot.ResultBlotDate:dd-MM-yyyy}, "; } catch { }
                    try { strAnalyzes += $"{_context.ListResults.First(e => e.ResultId == blot.ResultBlotResultId).ResultNameForRpt}; "; } catch { }
                }
                    

                foreach (var pcr in resPcr)
                {
                    try { strAnalyzes += $"ПЦР {pcr.ResultPcrDate:dd-MM-yyyy}, "; } catch { }
                    try { strAnalyzes += $"{_context.ListResults.First(e => e.ResultId == pcr.ResultPcrResultId).ResultNameForRpt}, {pcr.IntResultPcr} коп/мл;"; } catch { }
                }
                
                EditFile(path_from, i, fio, dateSex, residence, categery, lpuLab, item.NumIfa, strAnalyzes);
                i++;
            }
            ListNumForRptNotice num = new ListNumForRptNotice()
            {
                Num = _context.ListNumForRptNotices.OrderBy(e => e.Num).Last().Num + 1
            };
            _context.ListNumForRptNotices.Add(num);
            return PhysicalFile(path_from, file_type, file_name);
        }

        public static void CreateFile(string filepath, Reference_AIDSContext _context)
        {
            int num = _context.ListNumForRptNotices.OrderBy(e => e.Num).Last().Num + 1;
            string h1_1 = "ГБУЗ МО ЦПБ СПИД, клинико-диагностическая лаборатория",
                   h1_2 = "Московская область, г.о. Котельники, г. Котельники, мкр. Силикат, д. 41а",
                   h2_1 = "Форма № 266/У-88",
                   h2_2 = "Утверждена МЗ СССР 05.08.88 №690",
                   h3 = $"ОПЕРАТИВНОЕ ДОНЕСЕНИЕ № {num} " +
                        $"о лицах, в крови которых обнаружены маркеры ВИЧ," +
                        $" в эпидемиологический отдел Московского областного центра СПИД от референс подразделения КДЛ МОЦ СПИД";

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
                TableCell tc8 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2900" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Дата постановки, Результат"))));
                tr.Append(tc8);
                //TableCell tc10 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "950" }),
                //                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                //                                  new Run(new RunProperties(
                //                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                //                                              new FontSize { Val = new StringValue("20") },
                //                                              new Bold()),
                //                                          new Text("Примечание"))));
                //tr.Append(tc10);
                table.Append(tr);
                body.Append(table);
            }
            _context.ListNumForRptNotices.Add(new ListNumForRptNotice { Num = num });
            _context.SaveChanges();
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
                //TableCell tc10_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                //                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                //                                    new Run(new RunProperties(
                //                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                //                                                new FontSize { Val = new StringValue("20") }),
                //                                            new Text())));
                //tr2.Append(tc10_2);
                table.Append(tr2);
            }
        }
    }
}
