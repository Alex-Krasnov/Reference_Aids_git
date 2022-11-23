using Microsoft.AspNetCore.Mvc;
using Reference_Aids.ModelsForInput;
using Newtonsoft.Json.Linq;
using Reference_Aids.Data;
using Reference_Aids.ViewModels;
using System.IO.Pipelines;
using Reference_Aids.Models;

namespace Reference_Aids.Controllers
{
    public class ImportPhotometrController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public ImportPhotometrController(Reference_AIDSContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string reportId, string testSystem, string typeAnalyzes)
        {
            int testSystemId = _context.ListTestSystems.Where(e => e.TestSystemName == testSystem).First().TestSystemId;
            HttpClient client = new();
            var text = await client.GetStringAsync("http://169.254.76.239/api/report_gen/preview/" + reportId);
            var json = JObject.Parse(text);

            var CutOff = json["formulas"].Where(e => e["name"].Value<string>() == "ОПкрит").Select(e => e["calculated"]).First()
                                         .Select(e => e["Result"].Value<float>()).First();
            
            var layout = json["layout"].Where(e => e["barcode"].Value<string>() != "").ToList();
            var rawResult = layout.Select(e => new { id = e["barcode"].Value<int>(), res = e["od"].Value<float>() }).ToList();

            List<PhotometrResult> result = new();
            switch (typeAnalyzes)
            {
                case "ИФА":
                    foreach (var item in rawResult)
                    {
                        string resultStr;
                        // положитнльный если оп >= cutoff и на оборот
                        if (item.res >= CutOff)
                        {
                            resultStr = "+";
                        }
                        else
                        {
                            resultStr = "-";
                        }
                        PhotometrResult photometrResult = new()
                        {
                            PatientId = item.id,
                            PatientResult = item.res,
                            Result = resultStr
                        };
                        result.Add(photometrResult);
                    }
                    break;
                case "Антиген":
                    foreach (var item in rawResult)
                    {
                        string resultStr;
                        // положительный для обычного антигена если оп >= ОП крит.
                        if (item.res >= CutOff)
                        {
                            resultStr = "+";
                        }
                        else
                        {
                            resultStr = "-";
                        }
                        PhotometrResult photometrResult = new()
                        {
                            PatientId = item.id,
                            PatientResult = item.res,
                            Result = resultStr
                        };
                        result.Add(photometrResult);
                    }
                    break;
                case "Подтв ag":
                    foreach (var item in rawResult)
                    {
                        string resultStr;
                        var percentGash = ((item.res - CutOff) / item.res) * 100;
                        // положительный для подтв антигена если оп >= оп крит И percentGash >= 50%
                        if (item.res >= CutOff && percentGash >= 50)
                        {
                            resultStr = "+";
                        }
                        else
                        {
                            resultStr = "-";
                        }
                        PhotometrResult photometrResult = new()
                        {
                            PatientId = item.id,
                            PatientResult = item.res,
                            Result = resultStr
                        };
                        result.Add(photometrResult);
                    }
                    break;
                default:
                    return RedirectToAction("Index", "ImportAnalyzes");
            }

            var Viewdata = new ListForImportPhotometr
            {
                TestSystem = testSystem,
                TypeAnalyze = typeAnalyzes,
                CutOff = CutOff,
                ListPhotometrResult = result.OrderBy(e => e.PatientId)
            };

            ViewBag.Title = "ImportPhotometr";
            return View(Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> Add(List<InputAnalyzes> list)
        {
            List<string> ErrList = new();
            if (ModelState.IsValid)
            {
                DateOnly dateNow = DateOnly.FromDateTime(DateTime.Today);
                switch (list.First().TypeAnalyze)
                {
                    case "ИФА":
                        List<TblResultIfa> listIfa = new();
                        foreach (var item in list)
                        {
                            int? bloodId = null, resultIfaTestSysId = null;

                            try 
                            { 
                                bloodId = _context.TblIncomingBloods.First(e => e.NumIfa == item.PatientId && e.DateBloodImport.Year == dateNow.Year).BloodId; 
                            } 
                            catch 
                            {
                                ErrList.Add($"Не найден Рег. ном.:{item.PatientId}");
                                continue;
                            }
                            try
                            {
                                resultIfaTestSysId = _context.ListTestSystems.Where(e => e.TestSystemName == item.TestSystem).First().TestSystemId;
                            }
                            catch
                            {
                                ErrList.Add($"Не найдена тест система:{item.TestSystem}");
                                continue;
                            }

                            TblResultIfa tblResultIfa = new()
                            {
                                BloodId = bloodId,
                                ResultIfaDate = dateNow,
                                ResultIfaTestSysId = resultIfaTestSysId,
                                ResultIfaCutOff = item.CutOff,
                                ResultIfaOp = item.PatientResult,
                                ResultIfaResultId = _context.ListResults.Where(e => e.ResultName == item.Result).First().ResultId
                            };
                            listIfa.Add(tblResultIfa);
                            //_context.TblResultIfas.Add(tblResultIfa);
                            //await _context.SaveChangesAsync();
                        }
                        foreach(var ifa in listIfa)
                        {
                            _context.TblResultIfas.Add(ifa);
                            await _context.SaveChangesAsync();
                        }
                            
                        break;
                    case "Антиген":
                        List<TblResultAntigen> listAntigen = new();
                        foreach (var item in list)
                        {
                            int? bloodId = null, resultIfaTestSysId = null;

                            try
                            {
                                bloodId = _context.TblIncomingBloods.First(e => e.NumIfa == item.PatientId && e.DateBloodImport.Year == dateNow.Year).BloodId;
                            }catch
                            {
                                ErrList.Add($"Не найден Рег. ном.:{item.PatientId}");
                                continue;
                            }
                            try
                            {
                                resultIfaTestSysId = _context.ListTestSystems.Where(e => e.TestSystemName == item.TestSystem).First().TestSystemId;
                            }catch
                            {
                                ErrList.Add($"Не найдена тест система:{item.TestSystem}");
                                continue;
                            }
                            TblResultAntigen tblResultAntigen = new()
                            {
                                BloodId = bloodId,
                                ResultAntigenDate = dateNow,
                                ResultAntigenTestSysId = resultIfaTestSysId,
                                ResultAntigenCutOff = item.CutOff,
                                ResultAntigenOp = item.PatientResult,
                                ResultAntigenTypeId = 0,
                                ResultAntigenResultId = _context.ListResults.Where(e => e.ResultName == item.Result).First().ResultId
                            };
                            listAntigen.Add(tblResultAntigen);
                            //_context.TblResultAntigens.Add(tblResultAntigen);
                            //await _context.SaveChangesAsync();
                        }
                        foreach (var antigen in listAntigen)
                        {
                            _context.TblResultAntigens.Add(antigen);
                            await _context.SaveChangesAsync();
                        }
                            
                        break;
                    case "Подтв ag":
                        List<TblResultAntigen> listAntigenP = new();
                        foreach (var item in list)
                        {
                            int? bloodId = null, resultIfaTestSysId = null;

                            try
                            {
                                bloodId = _context.TblIncomingBloods.First(e => e.NumIfa == item.PatientId && e.DateBloodImport.Year == dateNow.Year).BloodId;
                            }catch
                            {
                                ErrList.Add($"Не найден Рег. ном.:{item.PatientId}");
                                continue;
                            }
                            try
                            {
                                resultIfaTestSysId = _context.ListTestSystems.Where(e => e.TestSystemName == item.TestSystem).First().TestSystemId;
                            }catch
                            {
                                ErrList.Add($"Не найдена тест система:{item.TestSystem}");
                                continue;
                            }
                            TblResultAntigen tblResultAntigen = new()
                            {
                                BloodId = bloodId,
                                ResultAntigenDate = dateNow,
                                ResultAntigenTestSysId = resultIfaTestSysId,
                                ResultAntigenCutOff = item.CutOff,
                                ResultAntigenOp = item.PatientResult,
                                ResultAntigenTypeId = 1,
                                ResultAntigenResultId = _context.ListResults.Where(e => e.ResultName == item.Result).First().ResultId
                            };
                            listAntigenP.Add(tblResultAntigen);
                            //_context.TblResultAntigens.Add(tblResultAntigen);
                            //await _context.SaveChangesAsync();
                        }
                        foreach (var antigen in listAntigenP)
                        {
                            _context.TblResultAntigens.Add(antigen);
                            await _context.SaveChangesAsync();
                        }
                            
                        break;
                    default:
                        return RedirectToAction("Index", "ImportAnalyzes");
                }
            }
            if (ErrList != null)
                return RedirectToAction("Index", "Error", new { list = ErrList });

            //await _context.SaveChangesAsync();
            return RedirectToAction("Index", "ImportAnalyzes");
        }
    }
}