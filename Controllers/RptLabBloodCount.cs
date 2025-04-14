using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using Reference_Aids.Models;
using AspNetCoreGeneratedDocument;

namespace Reference_Aids.Controllers
{
    public class RptLabBloodCount : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public RptLabBloodCount(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        [HttpPost]
        public IActionResult Create(string dat1, string dat2)
        {
            string path_from = _appEnvironment.WebRootPath + @$"\Files\Output\BloodCount_{DateTime.Now:dd_MM_yyyy}.docx",
            file_type = "text/plain",
            file_name = "BloodCount.docx";

            DateOnly date_start = DateOnly.Parse(dat1);
            DateOnly date_end = DateOnly.Parse(dat2);

            FileInfo fileInf1 = new(path_from);
            if (fileInf1.Exists)
                fileInf1.Delete();

            CreateFile(path_from, _context);

            var lisForInput = _context.TblIncomingBloods
                .Where(e => e.DateBloodImport.CompareTo(date_start) >= 0 && e.DateBloodImport.CompareTo(date_end) <= 0)
                .GroupBy(e => e.SendLabId)
                .Select(g => new
                {
                    SendLabId = g.Key,
                    Count = g.Count()
                })
                .ToList();

            foreach (var item in lisForInput)
            {
                var name = _context.ListSendLabs.First(e => e.SendLabId == item.SendLabId).SendLabName;
                EditFile(path_from, name, item.Count);
            }

            return PhysicalFile(path_from, file_type, file_name);
        }

        public static void CreateFile(string filepath, Reference_AIDSContext _context)
        {
            int num = _context.ListNumForRptNotices.OrderBy(e => e.Num).Last().Num + 1;

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                Table table = new(new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 })));

                TableRow tr = new();

                TableCell tc1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "5000" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Контрагент"))));
                tr.Append(tc1);
                TableCell tc2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1000" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Кол-во"))));
                tr.Append(tc2);
                
                table.Append(tr);
                body.Append(table);
            }
            _context.ListNumForRptNotices.Add(new ListNumForRptNotice { Num = num });
            _context.SaveChanges();
        }

        public static void EditFile(string filepath, string name, int count)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;
                Table table = body.Elements<Table>().First();
                TableRow tr2 = new();

                TableCell tc1_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(name))));
                tr2.Append(tc1_2);
                TableCell tc2_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                    new Run(new RunProperties(
                                                                new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                new FontSize { Val = new StringValue("20") }),
                                                            new Text(count.ToString()) { Space = SpaceProcessingModeValues.Preserve })));
                tr2.Append(tc2_2);
                table.Append(tr2);
            }
        }
    }
}
