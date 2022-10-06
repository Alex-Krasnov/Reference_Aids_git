using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using Reference_Aids.ModelsForInput;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Reference_Aids.Controllers
{
    public class InputPatientsController : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public InputPatientsController(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "InputPatient";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            List<InputPatients> dataList = new();

            if (uploadedFile != null)
            {
                string path = _appEnvironment.WebRootPath + "/Files/ForInput/" + uploadedFile.FileName;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                dataList = GetInputPatients(path, _context);
            }
            ViewBag.Title = "InputPatient";
            return View("RawImport", dataList);
        }

        public static List<InputPatients> GetInputPatients(string path, Reference_AIDSContext _context)
        {
            List<InputPatients> list = new();

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(path, true))
            {
                IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                var rows = sheetData.Elements<Row>();

                foreach (var row in rows)
                {
                    if (row.RowIndex == 1)
                        continue;
                    string familyName = row.Elements<Cell>().First(c => c.CellReference == "F" + row.RowIndex).InnerText;
                    InputPatients patients = new()
                    {
                        SendLab = row.Elements<Cell>().First(c => c.CellReference == "A"+ row.RowIndex).InnerText,
                        SendDistrict = row.Elements<Cell>().First(c => c.CellReference == "B" + row.RowIndex).InnerText,
                        DateBloodSampling = row.Elements<Cell>().First(c => c.CellReference == "C" + row.RowIndex).InnerText,
                        Category = row.Elements<Cell>().First(c => c.CellReference == "D" + row.RowIndex).InnerText,
                        Anon = row.Elements<Cell>().First(c => c.CellReference == "E" + row.RowIndex).InnerText,
                        FamilyName = row.Elements<Cell>().First(c => c.CellReference == "F" + row.RowIndex).InnerText,
                        FirstName = row.Elements<Cell>().First(c => c.CellReference == "G" + row.RowIndex).InnerText,
                        ThirdName = row.Elements<Cell>().First(c => c.CellReference == "H" + row.RowIndex).InnerText,
                        BirthDate = row.Elements<Cell>().First(c => c.CellReference == "I" + row.RowIndex).InnerText,
                        Sex = row.Elements<Cell>().First(c => c.CellReference == "J" + row.RowIndex).InnerText,
                        Phone = row.Elements<Cell>().First(c => c.CellReference == "K" + row.RowIndex).InnerText,
                        RegionName = row.Elements<Cell>().First(c => c.CellReference == "L" + row.RowIndex).InnerText,
                        CityName = row.Elements<Cell>().First(c => c.CellReference == "M" + row.RowIndex).InnerText,
                        AreaName = row.Elements<Cell>().First(c => c.CellReference == "N" + row.RowIndex).InnerText,
                        AddrStreat = row.Elements<Cell>().First(c => c.CellReference == "O" + row.RowIndex).InnerText,
                        AddrHome = row.Elements<Cell>().First(c => c.CellReference == "P" + row.RowIndex).InnerText,
                        AddrCorps = row.Elements<Cell>().First(c => c.CellReference == "Q" + row.RowIndex).InnerText,
                        AddrFlat = row.Elements<Cell>().First(c => c.CellReference == "R" + row.RowIndex).InnerText,
                        Blotdate = row.Elements<Cell>().First(c => c.CellReference == "S" + row.RowIndex).InnerText,
                        TestSys = row.Elements<Cell>().First(c => c.CellReference == "T" + row.RowIndex).InnerText,
                        CutOff = row.Elements<Cell>().First(c => c.CellReference == "U" + row.RowIndex).InnerText,
                        Result = row.Elements<Cell>().First(c => c.CellReference == "V" + row.RowIndex).InnerText,
                        PossiblePatients = _context.TblPatientCards.Where(e => e.FamilyName == familyName).ToList()
                    };
                    list.Add(patients);
                }
            }
            return list;
        }
    }
}
