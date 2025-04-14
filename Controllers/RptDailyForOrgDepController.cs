using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class RptDailyForOrgDepController : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public RptDailyForOrgDepController(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        [HttpPost]
        public IActionResult Index(string dat1, string dat2)
        {
            string path_to = _appEnvironment.WebRootPath + @$"\Files\Output\rptForOrgDep_{DateTime.Now:dd_MM_yyyy}.xlsx",
            path_from = _appEnvironment.WebRootPath + @"\Files\Sample\rptForOrgDep.xlsx",
            file_type = "text/plain";
            var file_name = "rptForOrgDep.xlsx";

            FileInfo fileInf1 = new(path_to);
            if (fileInf1.Exists)
                fileInf1.Delete();

            CreateFile(path_to);

            var lisForInput = (from patient in _context.TblPatientCards
                               join incBlood in _context.TblIncomingBloods on patient.PatientId equals incBlood.PatientId
                               select new
                               {
                                   patient.FamilyName,                                  // +
                                   patient.FirstName,                                   // +
                                   patient.ThirdName,                                   // +
                                   patient.BirthDate,                                   // +
                                   //patient.SexId, //+1 к sex_id  для конечного файла    // 
                                   incBlood.DateBloodImport,                            // -
                                   incBlood.NumIfa,                                     // +
                                   incBlood.BloodId                                     // +
                               }).Where(e => e.DateBloodImport >= DateOnly.Parse(dat1)
                                          && e.DateBloodImport <= DateOnly.Parse(dat2))
                               .OrderBy(e => e.NumIfa).ToList();
            //_context.SaveChangesAsync();
            List<ForXls> forXls = new();
            int i = 1;
            foreach (var item in lisForInput)
            {
                forXls.Add(
                    new ForXls { 
                        NumIfa=item.NumIfa, 
                        FamilyName=item.FamilyName, 
                        FirstName=item.FirstName,
                        ThirdName=item.ThirdName,
                        BirthDate=item.BirthDate,
                        BloodId=item.BloodId,
                        Num_row = i }
                    );

                //EditFile(path_to, item.NumIfa, item.FamilyName, item.FirstName, item.ThirdName, item.BirthDate, item.BloodId, i);

                TblIncomingBlood forUpd = _context.TblIncomingBloods.Where(e => e.BloodId == item.BloodId).First();
                forUpd.Repeat = false;
                _context.SaveChanges();

                i++;
            }
            EditFile(path_to, forXls);

            return PhysicalFile(path_to, file_type, file_name);
        }

        public static void EditFile(string filepath, List<ForXls> forXls)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true))
            {
                IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Sheet1");
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();

                foreach (var a in forXls)
                {
                    Row row = new Row() { RowIndex = (uint)a.Num_row };
                    sheetData.Append(row);
                    Cell refCell = null;

                    Cell cell_1 = new Cell() { CellReference = 1 + ":" + row.RowIndex.ToString() };
                    row.InsertBefore(cell_1, refCell);
                    cell_1.CellValue = new CellValue(a.FamilyName);
                    cell_1.DataType = CellValues.String;

                    Cell cell_2 = new Cell() { CellReference = 2 + ":" + row.RowIndex.ToString() };
                    row.InsertBefore(cell_2, refCell);
                    cell_2.CellValue = new CellValue(a.FirstName);
                    cell_2.DataType = CellValues.String;

                    Cell cell_3 = new Cell() { CellReference = 3 + ":" + row.RowIndex.ToString() };
                    row.InsertBefore(cell_3, refCell);
                    cell_3.CellValue = new CellValue(a.ThirdName);
                    cell_3.DataType = CellValues.String;

                    Cell cell_4 = new Cell() { CellReference = 4 + ":" + row.RowIndex.ToString() };
                    row.InsertBefore(cell_4, refCell);
                    cell_4.DataType = CellValues.Date; // Date
                    cell_4.CellValue = new CellValue(DateTime.Parse(a.BirthDate.ToString()));

                    Cell cell_5 = new Cell() { CellReference = 5 + ":" + row.RowIndex.ToString() };
                    row.InsertBefore(cell_5, refCell);
                    cell_5.CellValue = new CellValue(a.BloodId);
                    cell_5.DataType = CellValues.Number;

                    Cell cell_6 = new Cell() { CellReference = 6 + ":" + row.RowIndex.ToString() };
                    row.InsertBefore(cell_6, refCell);
                    cell_6.CellValue = new CellValue(a.NumIfa);
                    cell_6.DataType = CellValues.Number;
                }
                worksheetPart.Worksheet.Save();
            }
        }
        //public static void EditFile(string filepath, int numIfa, string familyName, string firstName, string thirdName, DateOnly birthDate, int bloodId, int num_row)
        //{
        //    using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true))
        //    {
        //        IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Sheet1");
        //        WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
        //        Worksheet worksheet = worksheetPart.Worksheet;
        //        SheetData sheetData = worksheet.GetFirstChild<SheetData>();

        //        Row row = new Row() { RowIndex = (uint)num_row };
        //        sheetData.Append(row);
        //        Cell refCell = null;

        //        Cell cell_1 = new Cell() { CellReference = 1 + ":" + row.RowIndex.ToString() };
        //        row.InsertBefore(cell_1, refCell);
        //        cell_1.CellValue = new CellValue(familyName);
        //        cell_1.DataType = CellValues.String;

        //        Cell cell_2 = new Cell() { CellReference = 2 + ":" + row.RowIndex.ToString() };
        //        row.InsertBefore(cell_2, refCell);
        //        cell_2.CellValue = new CellValue(firstName);
        //        cell_2.DataType = CellValues.String;

        //        Cell cell_3 = new Cell() { CellReference = 3 + ":" + row.RowIndex.ToString() };
        //        row.InsertBefore(cell_3, refCell);
        //        cell_3.CellValue = new CellValue(thirdName);
        //        cell_3.DataType = CellValues.String;

        //        Cell cell_4 = new Cell() { CellReference = 4 + ":" + row.RowIndex.ToString() };
        //        row.InsertBefore(cell_4, refCell);
        //        cell_4.DataType = CellValues.Date; // Date
        //        cell_4.CellValue = new CellValue(DateTime.Parse(birthDate.ToString()));



        //        Cell cell_5 = new Cell() { CellReference = 5 + ":" + row.RowIndex.ToString() };
        //        row.InsertBefore(cell_5, refCell);
        //        cell_5.CellValue = new CellValue(bloodId);
        //        cell_5.DataType = CellValues.Number;

        //        Cell cell_6 = new Cell() { CellReference = 6 + ":" + row.RowIndex.ToString() };
        //        row.InsertBefore(cell_6, refCell);
        //        cell_6.CellValue = new CellValue(numIfa);
        //        cell_6.DataType = CellValues.Number;

        //        worksheetPart.Worksheet.Save();
        //    }
        //}

        public static void CreateFile(string filepath)
        {
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                Create(filepath, SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Sheet1"
            };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();
            spreadsheetDocument.Close();
        }

        public class ForXls
        {
            public int NumIfa { get; set; }
            public string FamilyName { get; set; }
            public string FirstName { get; set; }
            public string ThirdName { get; set; }
            public DateOnly BirthDate { get; set; }
            public int BloodId { get; set; }
            public int Num_row { get; set; }
        }
    }
}
