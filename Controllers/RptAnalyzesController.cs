using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Reference_Aids.Models;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class RptAnalyzesController : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public RptAnalyzesController(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index(int ifaStart, int ifaEnd, string doctor, string dateId)
        {
            List<ListPatientForRpt> listPatient = new();
            var lisForInput = _context.TblPatientCards.Join(_context.TblIncomingBloods, p => p.PatientId, i => i.PatientId,
                                                           (p, i) => new {
                                                               p.FamilyName,
                                                               p.FirstName,
                                                               p.ThirdName,
                                                               p.BirthDate,
                                                               p.RegionId,
                                                               p.AddrHome,
                                                               p.AddrCorps,
                                                               p.AddrFlat,
                                                               p.AddrStreat,
                                                               p.CityName,
                                                               p.AreaName,
                                                               i.CategoryPatientId,
                                                               i.DateBloodImport,
                                                               i.NumIfa,
                                                               i.BloodId,
                                                               p.PatientId,
                                                               i.SendLabId
                                                           })
                                                     .Where(e => e.NumIfa >= ifaStart && e.NumIfa <= ifaEnd
                                                              && e.DateBloodImport.Year == Int32.Parse(dateId)).OrderBy(e => e.NumIfa).ToList();
            foreach (var item in lisForInput)
            {
                string adddrFull = "", resFull = "";

                try { adddrFull = _context.ListSendLabs.First(e => e.SendLabId == item.SendLabId).SendLabName; } catch { }
                
                //try { adddrFull += $"ГО{item.AreaName}"; } catch { }
                //try { adddrFull += $" г.{item.CityName}"; } catch { }
                //try { adddrFull += $" ул.{item.AddrStreat}"; } catch { }
                //try { adddrFull += $" д.{item.AddrHome}"; } catch { }
                //try { adddrFull += $" к.{item.AddrCorps}"; } catch { }
                //try { adddrFull += $" кв.{item.AddrFlat}"; } catch { }

                var ifaList = _context.TblResultIfas.Select(e => new { e.BloodId, e.ResultIfaDate, e.ResultIfaResultId }).Where(e => e.BloodId == item.BloodId).ToList();
                foreach (var ifa in ifaList)
                    try { resFull += $"Ифа {_context.ListResults.First(e => e.ResultId == ifa.ResultIfaResultId).ResultName};";} catch { }

                var ibList = _context.TblResultBlots.Select(e => new { e.BloodId, e.ResultBlotResultId, e.ResultBlotDate }).Where(e => e.BloodId == item.BloodId).ToList();
                foreach (var ib in ibList)
                    try { resFull += $"ИБ {_context.ListResults.First(e => e.ResultId == ib.ResultBlotResultId).ResultName};";} catch { }

                var antigenList = _context.TblResultAntigens.Select(e => new { e.BloodId, e.ResultAntigenDate, e.ResultAntigenResultId }).Where(e => e.BloodId == item.BloodId).ToList();
                foreach (var antigen in antigenList)
                    try { resFull += $"Антиген {_context.ListResults.First(e => e.ResultId == antigen.ResultAntigenResultId).ResultName}";} catch { }

                var pcrList = _context.TblResultPcrs.Select(e => new { e.BloodId, e.ResultPcrDate, e.ResultPcrResultId, e.IntResultPcr}).Where(e => e.BloodId == item.BloodId).ToList();
                foreach (var pcr in pcrList)
                    try { resFull += $"Пцр {_context.ListResults.First(e => e.ResultId == pcr.ResultPcrResultId).ResultName}, {pcr.IntResultPcr} коп/мл;"; } catch { }

                ListPatientForRpt patient = new()
                {
                    NumIfa = item.NumIfa,
                    PatientId = item.PatientId,
                    FirstName = item.FirstName,
                    FamilyName = item.FamilyName,
                    ThirdName = item.ThirdName,
                    BirthDate = item.BirthDate.ToString("dd-MM-yyyy"),
                    CategoryPatientId = item.CategoryPatientId,
                    AddrFull = adddrFull,
                    ResFull = resFull
                };

                listPatient.Add(patient);
            }

            var Viewdata = new ListForRptAnalyzesViewModel 
            { 
                ListRecForRpts = _context.ListRecForRpts.ToList(),
                TblPatientCards = listPatient,
                IfaEnd = ifaEnd,
                IfaStart = ifaStart,
                Doctor = doctor,
                DateId = dateId
            };

            ViewBag.Title = "RptAnalyzes";
            return View("Index", Viewdata);
        }
        [HttpPost]
        public IActionResult Create(int ifaStart, int ifaEnd, string doctor, string dateId, List<ForCreateRptAnalazys> list)
        {
            string rec = "";
            string path_from = _appEnvironment.WebRootPath + @$"\Files\Output\ReportAnalyzes_{DateTime.Now:dd_MM_yyyy}.docx",
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
                                                            (p, i) => new { p.FamilyName, p.FirstName, p.ThirdName, p.Sex, 
                                                                            p.BirthDate, p.RegionId, p.AddrHome, p.AddrCorps,
                                                                            p.AddrFlat, p.AddrStreat, p.CityName, p.AreaName,
                                                                            i.CategoryPatientId, i.SendDistrictNavigation, 
                                                                            i.SendLabNavigation, i.NumInList, 
                                                                            i.DateBloodSampling, i.DateBloodImport, 
                                                                            i.NumIfa,  i.BloodId, p.PatientId, p.PatientCom })
                                                      .Where(e => e.NumIfa >= ifaStart && e.NumIfa <= ifaEnd
                                                               && e.DateBloodImport.Year == Int32.Parse(dateId)).ToList();
            int i = 1;
            foreach (var item in lisForInput)
            {
                string Addrstr;

                try { Addrstr = $"Регион: {_context.ListRegions.Where(e => e.RegionId == item.RegionId).First().RegionName} ГО: {item.AreaName} г.{item.CityName} ул.{item.AddrStreat} д.{item.AddrHome} к.{item.AddrCorps} кв.{item.AddrFlat}"; }
                catch { Addrstr = "null"; }

                rec = list.First(e => e.PatientId == item.PatientId).Rec;

                EditFile(path_from, item.FamilyName, item.FirstName, item.ThirdName, item.BirthDate, item.Sex, item.SendDistrictNavigation, item.SendLabNavigation, item.CategoryPatientId, item.DateBloodSampling, item.DateBloodImport, item.NumIfa, _context, item.BloodId, item.PatientId, rec, Addrstr, doctor, item.PatientCom);
                if (i % 2 == 0)
                    InputIndent(path_from);
                i++;
            }

            return PhysicalFile(path_from, file_type, file_name);
        }
        public static void EditFile(string filepath, string? FamilyName, string? FirstName, string? ThirdName, DateOnly BirthDate, 
                                    ListSex? Sex, ListSendDistrict? SendDistrictNav, ListSendLab? SendLabNav, int? CategoryPatientId,
                                    DateOnly DateBloodSampling, DateOnly DateBloodImport, int NumIfa, 
                                    Reference_AIDSContext _context, int bloodId, int patientId, string recommendations, string Addrstr, string doctor, string com)//Добавление содержимого
        {
            string sendLab, sexName, sendDistrict, birthDate, dateBloodSampling, dateBloodImport;

            try { sexName = Sex.SexNameShort; }
            catch { sexName = "null"; }

            try { sendLab = SendLabNav.SendLabName; } 
            catch { sendLab = "null"; }

            try { sendDistrict = SendDistrictNav.SendDistrictName; }
            catch { sendDistrict = "null"; }

            try { birthDate = BirthDate.ToString("dd-MM-yyyy"); }
            catch { birthDate = "null"; }

            try { dateBloodSampling = DateBloodSampling.ToString("dd-MM-yyyy"); }
            catch { dateBloodSampling = "null"; }

            try { dateBloodImport = DateBloodImport.ToString("dd-MM-yyyy"); }
            catch { dateBloodImport = "null"; }

            var ifaList = _context.TblResultIfas.Select(e =>new { e.BloodId, e.ResultIfaDate, e.ResultIfaTestSysId, e.ResultIfaResultId} ).Where(e => e.BloodId == bloodId).ToList();
            var ibList = _context.TblResultBlots.Select(e => new {  e.ResultBlotResultId,e.ResultBlotEnv160,e.ResultBlotEnv120,e.ResultBlotEnv41,e.ResultBlotGag55,e.ResultBlotGag40,e.ResultBlotGag2425,e.ResultBlotGag18,e.ResultBlotPol6866,e.ResultBlotPol5251,e.ResultBlotPol3431,e.ResultBlotHiv2105,e.ResultBlotHiv236,e.ResultBlotHiv0,e.BloodId,e.ResultBlotTestSysId,e.ResultBlotDate}).Where(e => e.BloodId == bloodId).ToList();
            var antigenList = _context.TblResultAntigens.Select(e => new {e.BloodId, e.ResultAntigenDate, e.ResultAntigenTestSysId,e.ResultAntigenResultId }).Where(e => e.BloodId == bloodId).ToList();
            var pcrList = _context.TblResultPcrs.Select(e => new {e.BloodId, e.ResultPcrDate, e.ResultPcrResultId , e.ResultPcrTestSysId, e.IntResultPcr}).Where(e => e.BloodId == bloodId).ToList();
            
            var oldIBList = (from patient in _context.TblPatientCards
                             join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                             join resIB in _context.TblResultBlots on incBlood.BloodId equals resIB.BloodId
                             select new
                             {
                                 patient.PatientId,
                                 resIB.ResultBlotDate,
                                 resIB.ResultBlotResultId,
                                 resIB.ResultBlotTestSysId
                             })
                           .Where(e => e.PatientId == patientId && e.ResultBlotDate.Year < DateTime.Now.Year).ToList();

            //Шапка
            string p1 = "ГБУЗ Московской Области «ЦЕНТР ПРОФИЛАКТИКИ И БОРЬБЫ СО СПИД»",
                   p2 = "Адрес: Московская область, г.о. Котельники, г. Котельники, мкр. Силикат, д. 41а";

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
                                          new Text("ЛПУ направившее биоматериал: ") { Space = SpaceProcessingModeValues.Preserve }));
                para4.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(sendDistrict))));

                //Рег# СПИД ФИО
                Paragraph para5 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("28") },
                                              new Bold()),
                                          new Text("Рег.№ СПИД: ") { Space = SpaceProcessingModeValues.Preserve }));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("28") },
                                          new Text(NumIfa+" ") { Space = SpaceProcessingModeValues.Preserve })));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("28") },
                                              new Bold()),
                                          new Text("ФИО: ") { Space = SpaceProcessingModeValues.Preserve }));
                para5.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("28") },
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
                                          new Text(birthDate + " ") { Space = SpaceProcessingModeValues.Preserve })));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Код контингента: ") { Space = SpaceProcessingModeValues.Preserve }));
                para6.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(CategoryPatientId.ToString()))));

                //Адрес
                Paragraph para12 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para12.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Адрес: ") { Space = SpaceProcessingModeValues.Preserve }));
                para12.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(Addrstr + " ") { Space = SpaceProcessingModeValues.Preserve })));

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
                                          new Text(dateBloodSampling + " ") { Space = SpaceProcessingModeValues.Preserve })));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Дата поступления: ") { Space = SpaceProcessingModeValues.Preserve }));
                para7.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(dateBloodImport))));

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
                                          new Text("Сыворотка/плазма крови"))));

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
                foreach (var item in ifaList)
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
                                                                       new Text(item.ResultIfaDate.ToString("dd-MM-yyyy")))));
                    trOp.Append(tcOp2);
                    string ifaTs = null;
                    try { ifaTs = _context.ListTestSystems.First(e => e.TestSystemId == item.ResultIfaTestSysId).TestSystemName + " " +
                            _context.ListTestSystems.First(e => e.TestSystemId == item.ResultIfaTestSysId).DTestSystemShelfLife; } catch { }
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text(ifaTs))));
                    trOp.Append(tcOp3);
                    string ifaRes = "";
                    try { ifaRes = _context.ListResults.First(e => e.ResultId == item.ResultIfaResultId).ResultName; } catch { }
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text(ifaRes))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                //ИБ
                foreach (var item in ibList)
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
                                                                       new Text(item.ResultBlotDate.ToString("dd-MM-yyyy")))));
                    trOp.Append(tcOp2);
                    string ibTs = null;
                    try {ibTs = _context.ListTestSystems.First(e => e.TestSystemId == item.ResultBlotTestSysId).TestSystemName + " " +
                            _context.ListTestSystems.First(e => e.TestSystemId == item.ResultBlotTestSysId).DTestSystemShelfLife; } catch { }
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text(ibTs))));
                    trOp.Append(tcOp3);
                    string blotRes = "";
                    try { blotRes = _context.ListResults.First(e => e.ResultId == item.ResultBlotResultId).ResultName; } catch { }
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text(blotRes))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                //Антиген P24
                foreach (var item in antigenList)
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
                                                                       new Text(item.ResultAntigenDate.ToString("dd-MM-yyyy")))));
                    trOp.Append(tcOp2);
                    string agTs = null;
                    try { agTs = _context.ListTestSystems.First(e => e.TestSystemId == item.ResultAntigenTestSysId).TestSystemName + " " +
                            _context.ListTestSystems.First(e => e.TestSystemId == item.ResultAntigenTestSysId).DTestSystemShelfLife; } catch { }
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text(agTs))));
                    trOp.Append(tcOp3);
                    string anRes = "";
                    try { anRes = _context.ListResults.First(e => e.ResultId == item.ResultAntigenResultId).ResultName; } catch { }
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text(anRes))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                //ПЦР
                foreach (var item in pcrList)
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
                                                                       new Text(item.ResultPcrDate.ToString("dd-MM-yyyy")))));
                    trOp.Append(tcOp2);
                    string pcrTs = null;
                    try {pcrTs = _context.ListTestSystems.First(e => e.TestSystemId == item.ResultPcrTestSysId).TestSystemName
                            + " " + _context.ListTestSystems.First(e => e.TestSystemId == item.ResultPcrTestSysId).DTestSystemShelfLife; } catch { }
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text(pcrTs))));
                    trOp.Append(tcOp3);

                    string pcrRes = "";
                    try { pcrRes =_context.ListResults.First(e => e.ResultId == item.ResultPcrResultId).ResultName +", "+ item.IntResultPcr + " коп/мл"; } catch { }
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text(pcrRes))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);
                }
                //Старый ИБ
                foreach (var item in oldIBList)
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
                                                                       new Text(item.ResultBlotDate.ToString("dd-MM-yyyy")))));
                    trOp.Append(tcOp2);
                    string oldIbTs = null;
                    try { oldIbTs = _context.ListTestSystems.First(e => e.TestSystemId == item.ResultBlotTestSysId).TestSystemName + " " +
                            _context.ListTestSystems.First(e => e.TestSystemId == item.ResultBlotTestSysId).DTestSystemShelfLife; } catch { }
                    TableCell tcOp3 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("14") }),
                                                                     new Text(oldIbTs))));
                    trOp.Append(tcOp3);
                    string resIb = "";
                    try { resIb = _context.ListResults.First(e => e.ResultId == item.ResultBlotResultId).ResultName; } catch { }
                    TableCell tcOp4 = new(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }),
                                                             new Run(new RunProperties(
                                                                         new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                         new FontSize { Val = new StringValue("20") }),
                                                                     new Text(resIb))));
                    trOp.Append(tcOp4);
                    table.Append(trOp);

                }
                body.Append(table);

                //Таблица ИБ
                foreach(var item in ibList)
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
                    string res160 = null;
                    try { res160 = _context.ListResults.First(e =>e.ResultId ==item.ResultBlotEnv160).ResultName; } catch { }
                    TableCell tc1_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res160))));
                    string res120 = null;
                    try { res120 = _context.ListResults.First(e => e.ResultId ==item.ResultBlotEnv120).ResultName; } catch { }
                    TableCell tc2_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res120))));
                    string res41 = null;
                    try { res41 = _context.ListResults.First(e => e.ResultId ==item.ResultBlotEnv41).ResultName; } catch { }
                    TableCell tc3_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res41))));
                    tr3.Append(tc1_3);
                    tr3.Append(tc2_3);
                    tr3.Append(tc3_3);
                    string res55 = null;
                    try { res55 = _context.ListResults.First(e => e.ResultId == item.ResultBlotGag55).ResultName; } catch { }
                    TableCell tc4_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res55))));
                    string res40 = null;
                    try { res40 = _context.ListResults.First(e => e.ResultId == item.ResultBlotGag40).ResultName; } catch { }
                    TableCell tc5_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res40))));
                    string res2425 = null;
                    try { res2425 = _context.ListResults.First(e => e.ResultId == item.ResultBlotGag2425).ResultName; } catch { }
                    TableCell tc6_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res2425))));
                    string res18 = null;
                    try { res18 = _context.ListResults.First(e => e.ResultId == item.ResultBlotGag18).ResultName; } catch { }
                    TableCell tc7_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res18))));
                    tr3.Append(tc4_3);
                    tr3.Append(tc5_3);
                    tr3.Append(tc6_3);
                    tr3.Append(tc7_3);

                    string res6866 = null;
                    try { res6866 = _context.ListResults.First(e => e.ResultId == item.ResultBlotPol6866).ResultName; } catch { }
                    TableCell tc8_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res6866))));
                    string res5251 = null;
                    try { res5251 = _context.ListResults.First(e => e.ResultId == item.ResultBlotPol5251).ResultName; } catch { }
                    TableCell tc9_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res5251))));
                    string res3431 = null;
                    try { res3431 = _context.ListResults.First(e => e.ResultId == item.ResultBlotPol3431).ResultName; } catch { }
                    TableCell tc10_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res3431))));
                    tr3.Append(tc8_3);
                    tr3.Append(tc9_3);
                    tr3.Append(tc10_3);

                    string res2105 = null;
                    try { res2105 = _context.ListResults.First(e => e.ResultId == item.ResultBlotHiv2105).ResultName; } catch { }
                    TableCell tc11_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res2105))));
                    string res236 = null;
                    try { res236 = _context.ListResults.First(e => e.ResultId == item.ResultBlotHiv236).ResultName; } catch { }
                    TableCell tc12_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res236))));
                    tr3.Append(tc11_3);
                    tr3.Append(tc12_3);
                    string res0 = null;
                    try { res0 = _context.ListResults.First(e => e.ResultId == item.ResultBlotHiv0).ResultName; } catch { }
                    TableCell tc13_3 = new(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1" }, new VerticalMerge() { Val = MergedCellValues.Restart }, new Justification() { Val = JustificationValues.Center }),
                                        new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" }, new Justification() { Val = JustificationValues.Center }),
                                                      new Run(new RunProperties(
                                                                  new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                                                  new FontSize { Val = new StringValue("20") }),
                                                              new Text(res0))));
                    tr3.Append(tc13_3);
                    tableIB.Append(tr3);

                    body.Append(tableIB);
                }

                Paragraph para16 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                para16.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                              new Bold()),
                                          new Text("Комментарии: ") { Space = SpaceProcessingModeValues.Preserve }));
                para16.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text(com))));

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
                                          new Text(recommendations))));
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
                para11.AppendChild(new Run(new RunProperties(
                                              new RunFonts() { Ascii = "Calibri (Body)", HighAnsi = "Calibri (Body)" },
                                              new FontSize { Val = new StringValue("20") },
                                          new Text($"                                                                                            Врач: {doctor} ______") { Space = SpaceProcessingModeValues.Preserve })));
                Paragraph para13 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                Paragraph para14 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
                Paragraph para15 = body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0" })));
            }
        }

        public static void InputIndent(string filepath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;

                body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
            }
        }
    }
}
