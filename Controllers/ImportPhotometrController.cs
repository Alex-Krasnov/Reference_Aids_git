using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Reference_Aids.ModelsForInput;
using System.Text.Json.Nodes;

namespace Reference_Aids.Controllers
{
    public class ImportPhotometrController : Controller
    {
        public async Task<IActionResult> Index()
        {
            HttpClient client = new();

            var json = await client.GetStringAsync("http://169.254.76.239/api/report_gen/preview/756");
            var a = JsonObject.Parse(json);
            var layaot = JsonObject.Parse(json)["layout"];
            //var response = await client.GetFromJsonAsync<List<InputPhotometr>>("http://169.254.76.239/api/report_gen/preview/756");//эhttp://169.254.76.239/report/756
            //response.EnsureSuccessStatusCode();  
            //string responseBody = await response.Content.ReadAsStringAsync();

            return View();
        }
    }
}
