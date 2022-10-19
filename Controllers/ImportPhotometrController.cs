using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Reference_Aids.ModelsForInput;
using System.Text.Json.Nodes;

namespace Reference_Aids.Controllers
{
    public class ImportPhotometrController : Controller
    {
        public async Task<IActionResult> Index(string id)
        {
            HttpClient client = new();
            id = "756";//


            var json = await client.GetStringAsync("http://169.254.76.239/api/report_gen/preview/" + id);// 
            var a = JsonObject.Parse(json);
            var layout = JsonObject.Parse(json)["layout"];

            return View();
        }
    }
}
