using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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
        public IActionResult Create()
        {
            string path_from = _appEnvironment.WebRootPath +@$"\Files\Output\ReportNotice_{DateTime.Now:dd_MM_yyyy}.docx",
            file_type = "text/plain",
            file_name = "DailyReport.docx";

            DateOnly date_now = DateOnly.Parse(DateTime.Now.ToString("dd-MM-yyyy"));

            FileInfo fileInf1 = new(path_from);
            if (fileInf1.Exists)
                fileInf1.Delete();

            CreateFile(path_from, _context);
            
            var lisForInput = (from patient in _context.TblPatientCards
                               join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                               from resIfa in _context.TblResultIfas.Where(resIfa => resIfa.BloodId == incBlood.BloodId).DefaultIfEmpty()
                               from resPcr in _context.TblResultPcrs.Where(resPcr => resPcr.BloodId == incBlood.BloodId).DefaultIfEmpty()
                               from resIb in _context.TblResultBlots.Where(resIb => resIb.BloodId == incBlood.BloodId).DefaultIfEmpty()
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
                                   ResultBlotDate = resIb != null ? resIb.ResultBlotDate : new DateOnly(),
                                   ResultBlotResult = resIb != null ? resIb.ResultBlotResult : null,
                                   ResultBlotResultId = resIb != null ? resIb.ResultBlotResultId : null,
                                   ResultIfaDate = resIfa != null ? resIfa.ResultIfaDate : new DateOnly(),
                                   ResultIfaResult = resIfa != null ? resIfa.ResultIfaResult : null,
                                   ResultIfaResultId = resIfa != null ? resIfa.ResultIfaResultId : null,
                                   ResultPcrDate = resPcr != null ? resPcr.ResultPcrDate : new DateOnly(),
                                   ResultPcrResult = resPcr != null ? resPcr.ResultPcrResult : null,
                                   ResultPcrResultId = resPcr != null ? resPcr.ResultPcrResultId : null
                               }).Where(e => e.DateBloodImport == date_now).ToList();

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

                try { dateSex += item.BirthDate.ToString("dd-MM-yyyy"); }
                catch { }
                try { dateSex += " ," + item.Sex.SexNameShort; }
                catch { }

                try { strAnalyzes += $"{item.ResultIfaDate:dd-MM-yyyy}, ИФА {item.ResultIfaResult.ResultNameForRpt}; "; }
                catch { }
                try { strAnalyzes += $"{item.ResultBlotDate:dd-MM-yyyy}, ИБ {item.ResultBlotResult.ResultNameForRpt}; "; }
                catch { }
                try { strAnalyzes += $"{item.ResultPcrDate:dd-MM-yyyy}, ПЦР {item.ResultPcrResult.ResultNameForRpt}; "; }
                catch { }

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
                TableCell tc6 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1000" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Дата постановки, Результат"))));
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
                                                            new Text(lpuLab))));
                tr2.Append(tc6_2);
                TableCell tc8_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(strAnalyzes))));
                tr2.Append(tc8_2);
                table.Append(tr2);
            }
        }
    }
}
