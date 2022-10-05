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
            if (uploadedFile != null)
            {
                string path = _appEnvironment.WebRootPath + "/Files/ForInput/" + uploadedFile.FileName;

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                var Viewdata = GetInputPatients(path);
            }
            ViewBag.Title = "InputPatient";
            return View("RawImport", Viewdata);
        }

        public static List<InputPatients> GetInputPatients(string path)
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

                    InputPatients patients = new()
                    {
                        SendLab = row.Elements<Cell>().Where(c => c.CellReference == "A"+ row.RowIndex).First().InnerText.ToString(),
                        SendDistrict = row.Elements<Cell>().Where(c => c.CellReference == "B" + row.RowIndex).First().InnerText.ToString(),
                        DateBloodSampling = row.Elements<Cell>().Where(c => c.CellReference == "C" + row.RowIndex).First().InnerText.ToString(),
                        Category = row.Elements<Cell>().Where(c => c.CellReference == "D" + row.RowIndex).First().InnerText.ToString(),
                        Anon = row.Elements<Cell>().Where(c => c.CellReference == "E" + row.RowIndex).First().InnerText.ToString(),
                        FamilyName = row.Elements<Cell>().Where(c => c.CellReference == "F" + row.RowIndex).First().InnerText.ToString(),
                        FirstName = row.Elements<Cell>().Where(c => c.CellReference == "G" + row.RowIndex).First().InnerText.ToString(),
                        ThirdName = row.Elements<Cell>().Where(c => c.CellReference == "H" + row.RowIndex).First().InnerText.ToString(),
                        BirthDate = row.Elements<Cell>().Where(c => c.CellReference == "I" + row.RowIndex).First().InnerText.ToString(),
                        Sex = row.Elements<Cell>().Where(c => c.CellReference == "J" + row.RowIndex).First().InnerText.ToString(),
                        Phone = row.Elements<Cell>().Where(c => c.CellReference == "K" + row.RowIndex).First().InnerText.ToString(),
                        RegionName = row.Elements<Cell>().Where(c => c.CellReference == "L" + row.RowIndex).First().InnerText.ToString(),
                        CityName = row.Elements<Cell>().Where(c => c.CellReference == "M" + row.RowIndex).First().InnerText.ToString(),
                        AreaName = row.Elements<Cell>().Where(c => c.CellReference == "N" + row.RowIndex).First().InnerText.ToString(),
                        AddrStreat = row.Elements<Cell>().Where(c => c.CellReference == "O" + row.RowIndex).First().InnerText.ToString(),
                        AddrHome = row.Elements<Cell>().Where(c => c.CellReference == "P" + row.RowIndex).First().InnerText.ToString(),
                        AddrCorps = row.Elements<Cell>().Where(c => c.CellReference == "Q" + row.RowIndex).First().InnerText.ToString(),
                        AddrFlat = row.Elements<Cell>().Where(c => c.CellReference == "R" + row.RowIndex).First().InnerText.ToString(),
                        Blotdate = row.Elements<Cell>().Where(c => c.CellReference == "S" + row.RowIndex).First().InnerText.ToString(),
                        TestSys = row.Elements<Cell>().Where(c => c.CellReference == "T" + row.RowIndex).First().InnerText.ToString(),
                        CutOff = row.Elements<Cell>().Where(c => c.CellReference == "U" + row.RowIndex).First().InnerText.ToString(),
                        Result = row.Elements<Cell>().Where(c => c.CellReference == "V" + row.RowIndex).First().InnerText.ToString(),
                    };

                    list.Add(patients);
                }
            }
            return list;
        }
    }
}
