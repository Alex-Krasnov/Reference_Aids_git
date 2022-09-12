using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Reference_Aids.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Create()
        {
            string path_to = @"C:\work\Reference_Aids\Files\Output\form4.xlsx";
            string path_from = @"C:\work\Reference_Aids\Files\Sample\form4.xlsx";
            string file_type = "text/plain";
            string file_name = "form4.xlsx";

            FileInfo fileInf = new(path_from);
            fileInf.CopyTo(path_to);

            // заполнение файла
            string fileName = @"C:\Users\Public\Documents\myCellEx.xlsx";

            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(path_to, true);

            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();

            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

            worksheetPart.Worksheet = new Worksheet();
            Worksheet worksheet = new Worksheet();
            SheetData sheetData = new SheetData();
            Row row = new Row();
            Cell cell = new Cell()
            {
                CellReference = "A1",
                DataType = CellValues.String,
                CellValue = new CellValue("Microsoft")
            };
            row.Append(cell);
            sheetData.Append(row);
            worksheet.Append(sheetData);
            worksheetPart.Worksheet = worksheet;

            // Close the document.  
            spreadsheetDocument.Close();

            return PhysicalFile(path_to, file_type, file_name);
        }

        public IActionResult Delete()
        {

            string path = @"C:\work\Reference_Aids\Files\Output\form4.xlsx";

            FileInfo fileInfel = new(path);
            fileInfel.Delete();

            return View();
            //return PhysicalFile(file_path, file_type, file_name);
        }
    }
}
