using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Reference_Aids.ModelsForInput;

namespace Reference_Aids.Controllers
{
    public class RptAnalyzesController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public RptAnalyzesController(Reference_AIDSContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
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

            EditFile(path_from);

            return PhysicalFile(path_from, file_type, file_name);
        }

        public static void EditFile(string filepath)//Добавление содержимого
        {
            //Шапка
            string p1 = "ГКУЗ МО «Центр по профилактике и борьбе со СПИДом и инфекционными заболеваниями»",
                   p2 = "Адрес: г.Москва, ул.Щепкина 61/2 корп.8, ст.11 тел.: 8(495) 681-38-10,  8(495) 681-37-17 Эл.почта: mz_centrspid@mosreg.ru";

            RptAnalyzesData rptData = new();

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
                                          new Text("Направившая лаборатория: ")));
                para3.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.SendLab))));

                //ЛПУ направившее сыворотку
                Paragraph para4 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para4.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("ЛПУ направившее сыворотку: ")));
                para4.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.SendLPU))));

                //Рег# СПИД ФИО
                Paragraph para5 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Рег.№ СПИД: ")));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.RegNum))));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("ФИО: ")));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.PatientFIO))));

                //Пол Дата рождения Код контингента
                Paragraph para6 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Пол: ")));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.PatientSex))));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Дата рождения: ")));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.BirthDate))));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Код контингента: ")));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.Category))));

                //Дата забора крови Дата поступления
                Paragraph para7 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Дата забора крови:")));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.DBloodSampling))));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Дата поступления:")));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(rptData.DBloodImport))));

                //Биоматериал
                Paragraph para8 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para8.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Биоматериал:")));
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

                //Заголовок таблица ИБ
                Paragraph para9 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center})));
                para9.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("ИБ")));

                //Таблица ИБ
                
            }
        }
    }
}
