using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;

namespace Reference_Aids.Controllers
{
    public class RptAnalyzesConteoller : Controller
    {
        private readonly Reference_AIDSContext _context;
        public RptAnalyzesConteoller(Reference_AIDSContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string path_to = @$"C:\work\Reference_Aids\Files\Output\ReportAnalyzes_{DateTime.Now:dd_MM_yyyy}.xlsx",
            file_type = "text/plain";
            var file_name = "ReportAnalyzes.docx";

            CreateFile();

            return PhysicalFile(path_to, file_type, file_name);
        }

        public static void CreateFile()
        {

        }
    }
}
