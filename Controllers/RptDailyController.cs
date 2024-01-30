using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Reference_Aids.Controllers
{
    public class RptDailyController : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public RptDailyController(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        [HttpPost]
        public IActionResult Create(string dat)
        {
            string path_from = _appEnvironment.WebRootPath +@$"\Files\Output\DailyReport_{DateTime.Now:dd_MM_yyyy}.docx",
            file_type = "text/plain",
            file_name = "DailyReport.docx";

            DateOnly date_now = DateOnly.Parse(dat);

            FileInfo fileInf1 = new(path_from);
            if (fileInf1.Exists)
                fileInf1.Delete();

            CreateFile(path_from, _context);
            
            var lisForInput = (from patient in _context.TblPatientCards
                               join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                               select new
                               {
                                   patient.FamilyName,
                                   patient.FirstName,
                                   patient.ThirdName,
                                   patient.BirthDate,
                                   patient.Sex,
                                   incBlood.DateBloodImport,
                                   incBlood.NumIfa,
                                   incBlood.CategoryPatientId,
                                   incBlood.BloodId,
                                   incBlood.SendDistrictId,
                                   incBlood.SendLabId
                               }).Where(e => e.DateBloodImport == date_now).OrderBy(e => e.NumIfa).ToList();

            int i = 0;
            foreach (var item in lisForInput)
            {
                string fio = "", dateSex = "", categery = "", lpuLab = "", strAnalyzes = "";

                try { fio += item.FamilyName; } 
                catch { }
                try { fio += " " + item.FirstName; }
                catch { }
                try { fio += " " + item.ThirdName; }
                catch { }

                try { categery =item.CategoryPatientId.ToString(); }
                catch { }

                try { dateSex += item.BirthDate.ToString("dd-MM-yyyy"); }
                catch { }
                try { dateSex += " ," + item.Sex.SexNameShort; }
                catch { }

                try { lpuLab +=_context.ListSendLabs.First(e => e.SendLabId == item.SendLabId).SendLabName; }
                catch { }

                var resIfa = _context.TblResultIfas.Where(e => e.BloodId == item.BloodId).ToList();
                var resPcr = _context.TblResultPcrs.Where(e => e.BloodId == item.BloodId).ToList();
                var resAntigen = _context.TblResultAntigens.Where(e => e.BloodId == item.BloodId).ToList();
                var resBlot = _context.TblResultBlots.Where(e => e.BloodId == item.BloodId).ToList();

                foreach (var ifa in resIfa)
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

                EditFile(path_from, item.NumIfa, fio, dateSex, categery, lpuLab, strAnalyzes);
                i++;
            }

            return PhysicalFile(path_from, file_type, file_name);
        }

        public static void CreateFile(string filepath, Reference_AIDSContext _context)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                //Paragraph para = 
                body.AppendChild(new SectionProperties(
                    new PageSize() { Width = (UInt32Value)15840U, Height = (UInt32Value)12240U, Orient = PageOrientationValues.Landscape },
                    new PageMargin() { Top = 708, Right = 1134, Bottom = 850, Left = 1440, Header = 450, Footer = 708 }));

                Table table = new(new TableProperties(
                    new SectionProperties(
                        new PageSize() { Width = (UInt32Value)15840U, Height = (UInt32Value)12240U, Orient = PageOrientationValues.Landscape },
                        new PageMargin() { Top = 720, Right = Convert.ToUInt32(1 * 1440.0), Bottom = 360, Left = Convert.ToUInt32(1 * 1440.0), Header = (UInt32Value)450U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U }),
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
                                                          new Text("Штрих код"))));
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
                                                          new Text("Код. заб."))));
                tr.Append(tc4);
                TableCell tc5 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "800" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Лаборатория"))));
                tr.Append(tc5);
                TableCell tc6 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "10000" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Анализы в базе"))));
                tr.Append(tc6);
                table.Append(tr);
                body.Append(table);
            }
        }

        public static void EditFile(string filepath, int numIfa, string fio, string dateSex, string category, string lpuLab, string strAnalyzes)
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
                                                            new Text(numIfa.ToString()))));
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
                                                            new Text(category) { Space = SpaceProcessingModeValues.Preserve })));
                tr2.Append(tc4_2);
                TableCell tc5_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(lpuLab))));
                tr2.Append(tc5_2);
                TableCell tc6_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa}),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(strAnalyzes))));
                tr2.Append(tc6_2);
                table.Append(tr2);
            }
        }
    }
}
