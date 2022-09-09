using Microsoft.AspNetCore.Mvc;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace Reference_Aids.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Index()
        {
            var wordApp = new Word.Application();
            //var excel = new Excel.Application();


            return View();
        }
    }
}
