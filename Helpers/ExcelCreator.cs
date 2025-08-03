using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace HIVBackend.Helpers
{
    public class ExcelCreator
    {
        public void CreateExcel(List<IDictionary<string, object>> data, string filePath, List<string> headers)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = doc.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = doc.WorkbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = doc.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Write headers
                Row headerRow = new Row();
                foreach (string header in headers)
                {
                    headerRow.AppendChild(CreateCell(header));
                }
                sheetData.AppendChild(headerRow);

                // Write data
                foreach (Dictionary<string, object> rowData in data)
                {
                    Row dataRow = new Row();
                    foreach (object value in rowData.Values)
                    {
                        dataRow.AppendChild(CreateCell(value?.ToString()));
                    }
                    sheetData.AppendChild(dataRow);
                }
            }
        }

        private Cell CreateCell(string value)
        {
            return new Cell(new CellValue(value)) { DataType = CellValues.String };
        }
    }
}
