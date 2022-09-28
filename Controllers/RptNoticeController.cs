using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class RptNoticeController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public RptNoticeController(Reference_AIDSContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult Create(string dat1, string dat2)
        {
            string path_from = @$"C:\work\Reference_Aids\Files\Output\ReportNotice_{DateTime.Now:dd_MM_yyyy}.docx",
            file_type = "text/plain",
            file_name = "ReportNotice.docx";

            FileInfo fileInf1 = new(path_from);
            if (fileInf1.Exists)
                fileInf1.Delete();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(path_from, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
            }


            return PhysicalFile(path_from, file_type, file_name);
        }

        public static void EditFile(string filepath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;

            }
        }
    }
}
