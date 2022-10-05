using Microsoft.AspNetCore.Mvc;

namespace Reference_Aids.Controllers
{
    public class InputPatientsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            IWebHostEnvironment _appEnvironment;

            if (uploadedFile != null)
            {
                //// путь к папке Files
                //string path = "/Files/" + uploadedFile.FileName;
                //// сохраняем файл в папку Files в каталоге wwwroot
                //using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                //{
                //    await uploadedFile.CopyToAsync(fileStream);
                //}
                //FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
                //_context.Files.Add(file);
                //_context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
