using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class RptAnalyzesController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public RptAnalyzesController(Reference_AIDSContext context)
        {
            _context = context;
        }

        public IActionResult Index(int ifaStart, int ifaEnd)
        {
            ifaStart = 0;
            ifaEnd = 10000;

            //string path_from = @$"C:\Users\alexk\source\repos\Alex-Krasnov\Reference_Aids_git\Files\Output\ReportAnalyzes_{DateTime.Now:dd_MM_yyyy}.docx",
            string path_from = @$"C:\work\Reference_Aids\Files\Output\ReportAnalyzes_{DateTime.Now:dd_MM_yyyy}.docx",
            file_type = "text/plain",
            file_name = "ReportAnalyzes.docx";

            FileInfo fileInf1 = new(path_from);
            if (fileInf1.Exists)
                fileInf1.Delete();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(path_from, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
            }

            var lisForInput = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                            (p, i) => new { p.FamilyName, p.FirstName, p.ThirdName, p.Sex, p.BirthDate, i.CategoryPatientId, i.SendDistrictNavigation, i.SendLabNavigation, i.NumInList, i.DateBloodSampling, i.DateBloodImport, i.NumIfa })
                                                      .Where(e => e.NumIfa >= ifaStart && e.NumIfa <= ifaEnd).ToList();
            int i = 1;
            foreach (var item in lisForInput)
            {
                if (i % 2 == 0)
                    InputIndent(path_from);

                EditFile(path_from, item.FamilyName, item.FirstName, item.ThirdName, item.BirthDate.ToString("dd-MM-yyyy"), item.Sex, item.SendDistrictNavigation, item.SendLabNavigation, item.CategoryPatientId, item.DateBloodSampling.ToString("dd-MM-yyyy"), item.DateBloodImport.ToString("dd-MM-yyyy"), item.NumIfa, item.NumInList);
                i++;
            }

            return PhysicalFile(path_from, file_type, file_name);
        }
        public static void EditFile(string filepath, string? FamilyName, string? FirstName, string? ThirdName, string? BirthDate, 
                                    ListSex? Sex, ListSendDistrict? SendDistrictNav, ListSendLab? SendLabNav, int? CategoryPatientId, 
                                    string DateBloodSampling, string DateBloodImport, int NumIfa, int NumInList)//Добавление содержимого
        {
            string sendLab = "null", sexName = "null", sendDistrict = "null";
            if (Sex != null)
                sexName = Sex.SexNameShort;
            if (SendLabNav != null)
                sendLab = SendLabNav.SendLabName;
            if (SendDistrictNav != null)
                sexName = SendDistrictNav.SendDistrictName;

            //Шапка
            string p1 = "ГКУЗ МО «Центр по профилактике и борьбе со СПИДом и инфекционными заболеваниями»",
                   p2 = "Адрес: г.Москва, ул.Щепкина 61/2 корп.8, ст.11 тел.: 8(495) 681-38-10,  8(495) 681-37-17 Эл.почта: mz_centrspid@mosreg.ru";

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, true))
            {

                Body body = wordDocument.MainDocumentPart.Document.Body;

                //Создание "шапки"
                Paragraph para1 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para1.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" }, 
                                              new FontSize { Val = new StringValue("14") }),
                                          new Text(p1)));
                
                Paragraph para2 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "160" })));
                para2.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" }, 
                                              new FontSize { Val = new StringValue("14") }),
                                          new Text(p2)));

                //Направившая лаборатория
                Paragraph para3 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para3.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" }, 
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Направившая лаборатория: ") { Space = SpaceProcessingModeValues.Preserve }));
                para3.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(sendLab))));

                //ЛПУ направившее сыворотку
                Paragraph para4 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para4.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("ЛПУ направившее сыворотку: ") { Space = SpaceProcessingModeValues.Preserve }));
                para4.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(sendDistrict))));

                //Рег# СПИД ФИО
                Paragraph para5 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Рег.№ СПИД: ") { Space = SpaceProcessingModeValues.Preserve }));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(NumIfa+" ") { Space = SpaceProcessingModeValues.Preserve })));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("ФИО: ") { Space = SpaceProcessingModeValues.Preserve }));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(FamilyName+" "+FirstName+" "+ThirdName) { Space = SpaceProcessingModeValues.Preserve })));

                //Пол Дата рождения Код контингента
                Paragraph para6 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Пол: ") { Space = SpaceProcessingModeValues.Preserve }));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(sexName + " ") { Space = SpaceProcessingModeValues.Preserve })));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Дата рождения: ") { Space = SpaceProcessingModeValues.Preserve }));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(BirthDate + " ") { Space = SpaceProcessingModeValues.Preserve })));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Код контингента: ") { Space = SpaceProcessingModeValues.Preserve }));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(CategoryPatientId.ToString()))));

                //Дата забора крови Дата поступления
                Paragraph para7 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Дата забора крови: ") { Space = SpaceProcessingModeValues.Preserve }));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(DateBloodSampling + " ") { Space = SpaceProcessingModeValues.Preserve })));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Дата поступления: ") { Space = SpaceProcessingModeValues.Preserve }));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(DateBloodImport))));

                //Биоматериал
                Paragraph para8 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para8.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Биоматериал: ") { Space = SpaceProcessingModeValues.Preserve }));
                para8.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text("Сыворотка/плазма"))));

                //Таблица Анализов
                Table table = new(new TableProperties(
                    new TableBorders(
                        new TopBorder(){Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines),Size = 0},
                        new BottomBorder(){Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines),Size = 0},
                        new LeftBorder(){Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines),Size = 0},
                        new RightBorder(){Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines),Size = 0},
                        new InsideHorizontalBorder(){Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines),Size = 0},
                        new InsideVerticalBorder(){Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines),Size = 0})));

                TableRow tr = new();

                TableCell tc1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1500" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Наименование теста"))));
                tr.Append(tc1);
                TableCell tc2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1000" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Дата"))));
                tr.Append(tc2);
                TableCell tc3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "6000" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Серия, Тест система"))));
                tr.Append(tc3);
                TableCell tc4 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "950" }),
                                    new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                  new Run(new RunProperties(
                                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                              new FontSize { Val = new StringValue("20") },
                                                              new Bold()),
                                                          new Text("Результат"))));
                tr.Append(tc4);
                table.Append(tr);
                //ИФА
                if (1 == 1) 
                {
                    TableRow trOp = new();
                    TableCell tcOp1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1500" }),
                                                 new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("ИФА"))));
                    trOp.Append(tcOp1);
                    TableCell tcOp2 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("01-01-1900"))));
                    trOp.Append(tcOp2);
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text("Серия, Тест система"))));
                    trOp.Append(tcOp3);
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text("Сомн."))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                //ИБ
                if (1 == 1) 
                {
                    TableRow trOp = new();
                    TableCell tcOp1 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("ИБ"))));
                    trOp.Append(tcOp1);
                    TableCell tcOp2 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("01-01-1900"))));
                    trOp.Append(tcOp2);
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text("Серия, Тест система"))));
                    trOp.Append(tcOp3);
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text("Отр."))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                //Антиген P24
                if (1 == 1)
                {
                    TableRow trOp = new();
                    TableCell tcOp1 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("Антиген P24"))));
                    trOp.Append(tcOp1);
                    TableCell tcOp2 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("01-01-1900"))));
                    trOp.Append(tcOp2);
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text("Серия, Тест система"))));
                    trOp.Append(tcOp3);
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text("Отр."))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                //ПЦР
                if (1 == 1)
                {
                    TableRow trOp = new();
                    TableCell tcOp1 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("ПЦР"))));
                    trOp.Append(tcOp1);
                    TableCell tcOp2 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("01-01-1900"))));
                    trOp.Append(tcOp2);
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text("Серия, Тест система"))));
                    trOp.Append(tcOp3);
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text("Отр."))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                //Старый ИБ
                if (1 == 1) 
                {
                    TableRow trOp = new();
                    TableCell tcOp1 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("ИБ ранее "))));
                    trOp.Append(tcOp1);
                    TableCell tcOp2 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                               new Run(new RunProperties(
                                                                           new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                           new FontSize { Val = new StringValue("20") }),
                                                                       new Text("01-01-1900"))));
                    trOp.Append(tcOp2);
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text("Серия, Тест система"))));
                    trOp.Append(tcOp3);
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text("Пол."))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                body.Append(table);

                //Таблица ИБ
                if (1==1)
                {
                    //Заголовок таблица ИБ
                    Paragraph para9 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center })));
                    para9.AppendChild(new Run(new RunProperties(
                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                  new FontSize { Val = new StringValue("20") },
                                                  new Bold()),
                                              new Text("ИБ")));

                    //Таблица ИБ
                    Table tableIB = new(new TableProperties(
                        new TableBorders(
                            new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                            new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                            new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                            new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                            new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 },
                            new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 0 }),
                        new TableWidth() { Width = "9390" }));
                    //ENV
                    TableRow tr1 = new();
                    TableCell tc1_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Restart }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") },
                                                                  new Bold()),
                                                              new Text("ENV"))));
                    TableCell tc2_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    TableCell tc3_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    tr1.Append(tc1_1);
                    tr1.Append(tc2_1);
                    tr1.Append(tc3_1);
                    //GAG
                    TableCell tc4_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Restart }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") },
                                                                  new Bold()),
                                                              new Text("GAG"))));
                    TableCell tc5_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    TableCell tc6_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    TableCell tc7_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    tr1.Append(tc4_1);
                    tr1.Append(tc5_1);
                    tr1.Append(tc6_1);
                    tr1.Append(tc7_1);
                    //POL
                    TableCell tc8_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Restart }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") },
                                                                  new Bold()),
                                                              new Text("POL"))));
                    TableCell tc9_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    TableCell tc10_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    tr1.Append(tc8_1);
                    tr1.Append(tc9_1);
                    tr1.Append(tc10_1);

                    //ВИЧ2
                    TableCell tc11_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Restart }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") },
                                                                  new Bold()),
                                                              new Text("ВИЧ2"))));
                    TableCell tc12_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new HorizontalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    tr1.Append(tc11_1);
                    tr1.Append(tc12_1);

                    //ВИЧ 0
                    TableCell tc13_1 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") },
                                                                  new Bold()),
                                                              new Text("ВИЧ 0"))));
                    tr1.Append(tc13_1);

                    TableRow tr2 = new();
                    TableCell tc1_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("160/140"))));
                    TableCell tc2_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("120"))));
                    TableCell tc3_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("41"))));
                    tr2.Append(tc1_2);
                    tr2.Append(tc2_2);
                    tr2.Append(tc3_2);

                    TableCell tc4_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("55/56"))));
                    TableCell tc5_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("40"))));
                    TableCell tc6_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("24/25/26"))));
                    TableCell tc7_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("18"))));
                    tr2.Append(tc4_2);
                    tr2.Append(tc5_2);
                    tr2.Append(tc6_2);
                    tr2.Append(tc7_2);

                    TableCell tc8_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("68/66"))));
                    TableCell tc9_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("52/51"))));
                    TableCell tc10_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("34/31"))));
                    tr2.Append(tc8_2);
                    tr2.Append(tc9_2);
                    tr2.Append(tc10_2);

                    TableCell tc11_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("105"))));
                    TableCell tc12_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("36"))));
                    tr2.Append(tc11_2);
                    tr2.Append(tc12_2);

                    TableCell tc13_2 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Continue }),
                                        new Paragraph());
                    tr2.Append(tc13_2);

                    tableIB.Append(tr1);
                    tableIB.Append(tr2);

                    TableRow tr3 = new();
                    TableCell tc1_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    TableCell tc2_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    TableCell tc3_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    tr3.Append(tc1_3);
                    tr3.Append(tc2_3);
                    tr3.Append(tc3_3);

                    TableCell tc4_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    TableCell tc5_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    TableCell tc6_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    TableCell tc7_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    tr3.Append(tc4_3);
                    tr3.Append(tc5_3);
                    tr3.Append(tc6_3);
                    tr3.Append(tc7_3);

                    TableCell tc8_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    TableCell tc9_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    TableCell tc10_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    tr3.Append(tc8_3);
                    tr3.Append(tc9_3);
                    tr3.Append(tc10_3);

                    TableCell tc11_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    TableCell tc12_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    tr3.Append(tc11_3);
                    tr3.Append(tc12_3);

                    TableCell tc13_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text("+"))));
                    tr3.Append(tc13_3);
                    tableIB.Append(tr3);

                    body.Append(tableIB);
                }

                //Рекомендовано
                Paragraph para10 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para10.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Рекомендовано: ") { Space = SpaceProcessingModeValues.Preserve }));
                para10.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text())));
                //Дата выдачи Врач
                Paragraph para11 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para11.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Дата выдачи: ") { Space = SpaceProcessingModeValues.Preserve }));
                para11.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(DateTime.Today.ToString("dd-MM-yyyy")))));
            }
        }

        public static void InputIndent(string filepath)//Ввод абзацев
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;

                body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
            }
        }
    }
}
