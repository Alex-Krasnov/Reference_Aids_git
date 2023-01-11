using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using Reference_Aids.ModelsForInput;
using Reference_Aids.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Reference_Aids.ViewModels;
using System.Drawing;
using System.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Reference_Aids.Controllers
{
    public class InputRepeateIncBloodController : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public InputRepeateIncBloodController(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "InputRepeateIncBlood";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = _appEnvironment.WebRootPath + @"\Files\ForInput\" + uploadedFile.FileName;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                GetInputPatients(path, _context);

                FileInfo fileInf1 = new(path);
                if (fileInf1.Exists)
                    fileInf1.Delete();
            }
            return RedirectToAction("Index");
        }
        public async void GetInputPatients(string path, Reference_AIDSContext _context)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(path, false))
            {
                IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                var rows = sheetData.Elements<Row>();
                int i = 0;

                //костыль переделать
                WorkbookPart wbPart = spreadsheetDocument.WorkbookPart;
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
                
                foreach (var row in rows)
                {
                    if (row.RowIndex == 1)
                        continue;

                    int bloodId = Int32.Parse(row.Elements<Cell>().First(c => c.CellReference == "E" + row.RowIndex).InnerText);
                    Boolean rep = false;
                    if (row.Elements<Cell>().First(c => c.CellReference == "G" + row.RowIndex).InnerText == "1")
                        rep = true;

                    TblIncomingBlood forUpd = _context.TblIncomingBloods.Where(e => e.BloodId == bloodId).First();
                    forUpd.Repeat = rep;
                    await _context.SaveChangesAsync();
                    i++;
                }
            }
        }
    }
}
