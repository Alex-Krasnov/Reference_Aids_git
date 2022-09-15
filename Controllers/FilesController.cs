using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Reference_Aids.ModelsForInput;
using Reference_Aids.Data;

namespace Reference_Aids.Controllers
{
    public class FilesController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public FilesController(Reference_AIDSContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string path_to = @$"C:\work\Reference_Aids\Files\Output\form4_{DateTime.Now:dd_MM_yyyy}.xlsx";
            string path_from = @"C:\work\Reference_Aids\Files\Sample\form4.xlsx";
            string file_type = "text/plain";
            var file_name = "form4.xlsx";

            FileInfo fileInf1 = new(path_to);
            if (fileInf1.Exists)
                fileInf1.Delete();

            FileInfo fileInf = new(path_from);
            fileInf.CopyTo(path_to);

            var a = ListforForm4();

            for (int i = 13; i < 35; i++ )
            {
                if (i == 16 || i == 25 || i == 30)
                    continue;

                foreach(char c in "DEFGHIJKLMNOP")
                    ChangeTextInCell(path_to, i, i, c);
            }
            return PhysicalFile(path_to, file_type, file_name);
        }

        public static void ChangeTextInCell(string filepath, int value, int num_row, char num_col)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true))
            {
                IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Sheet1");
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();

                Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == num_row).First();
                Cell cell = row.Elements<Cell>().Where(c => c.CellReference.Value == $"{num_col}{num_row}").First();
                cell.CellValue = new CellValue(value);
                cell.DataType = CellValues.Number;

                worksheetPart.Worksheet.Save();
            }
        }

        public List<Form4> ListforForm4()
        {
            var cat = _context.ListCategories.Select(e => e.CategoryId).ToList();
            List<Form4> form4;

            foreach (int item in cat)
            {
                new form4 (item, 100);
            }

            return form4;
        }
    }
}
