using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using Reference_Aids.ModelsForInput;
using Reference_Aids.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Reference_Aids.ViewModels;
using System.Drawing;
using System.Linq;
using System.Globalization;
using Microsoft.Build.Evaluation;

namespace Reference_Aids.Controllers
{
    public class InputPatientsController : Controller
    {
        private readonly Reference_AIDSContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public InputPatientsController(Reference_AIDSContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "InputPatient";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            ListForImportPatient dataList = new();

            if (uploadedFile != null)
            {
                string path = _appEnvironment.WebRootPath + @"\Files\ForInput\" + uploadedFile.FileName;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                dataList = GetInputPatients(path, _context);
            }
            ViewBag.Title = "InputPatient";
            
            return View("RawImport", dataList);
        }

        [HttpPost]
        public async Task<IActionResult> Input(List<InputPatients> Patients)
        {
            List<Success> successfulList = new();
            List<string> errList = new();

            var a = Request.Form;

            foreach (var patient in Patients)
            {
                int labId, districtId, testSystemId, regionId;
                //гарантирован ввод лаборатории, кем. напр., тест системы, региона
                try { labId = _context.ListSendLabs.First(e => e.SendLabName == patient.SendLab).SendLabId; } 
                catch 
                {
                    ListSendLab lab = new()
                    {
                        SendLabId = _context.ListSendLabs.Max(e => e.SendLabId) + 1,
                        SendLabName = patient.SendLab
                    };
                    _context.Add(lab);
                    await _context.SaveChangesAsync();
                    labId = _context.ListSendLabs.First(e => e.SendLabName == patient.SendLab).SendLabId;
                }

                try { districtId = _context.ListSendDistricts.First(e => e.SendDistrictName == patient.SendDistrict).SendDistrictId; }
                catch
                {
                    ListSendDistrict dist = new()
                    {
                        SendDistrictId = _context.ListSendDistricts.Max(e => e.SendDistrictId) + 1,
                        SendDistrictName = patient.SendDistrict
                    };
                    _context.Add(dist);
                    await _context.SaveChangesAsync();
                    districtId = _context.ListSendDistricts.First(e => e.SendDistrictName == patient.SendDistrict).SendDistrictId;
                }

                try { testSystemId = _context.ListTestSystems.First(e => e.TestSystemName == patient.TestSys).TestSystemId; }
                catch
                {
                    ListTestSystem testSystem = new()
                    {
                        TestSystemId = _context.ListTestSystems.Max(e => e.TestSystemId) + 1,
                        TestSystemName = patient.TestSys
                    };
                    _context.Add(testSystem);
                    await _context.SaveChangesAsync();
                    testSystemId = _context.ListTestSystems.First(e => e.TestSystemName == patient.TestSys).TestSystemId;
                }

                try { regionId = _context.ListRegions.First(e => e.RegionName == patient.RegionName).RegionId; }
                catch
                {
                    ListRegion region = new()
                    {
                        RegionId = _context.ListRegions.Max(e => e.RegionId) + 1,
                        RegionName = patient.RegionName
                    };
                    _context.Add(region);
                    await _context.SaveChangesAsync();
                    regionId = _context.ListRegions.First(e => e.RegionName == patient.RegionName).RegionId;
                }

                if (patient.PatienId != null)
                {
                    try
                    {
                        TblPatientCard tblPatientCard1 = new()
                        {
                            PatientId = Int32.Parse(patient.PatienId),
                            FamilyName = patient.FamilyName,
                            FirstName = patient.FirstName,
                            ThirdName = patient.ThirdName,
                            BirthDate = DateOnly.Parse(patient.BirthDate),
                            SexId = _context.ListSexes.First(e => e.SexNameShort == patient.Sex).SexId,
                            RegionId = regionId,
                            CityName = patient.CityName,
                            AreaName = patient.AreaName,
                            PhoneNum = patient.Phone,
                            AddrHome = patient.AddrHome,
                            AddrCorps = patient.AddrCorps,
                            AddrFlat = patient.AddrFlat,
                            AddrStreat = patient.AddrStreat,
                            PatientCom = patient.PatientCom
                        };
                        _context.TblPatientCards.Update(tblPatientCard1);

                        double cutOff, res, blotKP;
                        NumberFormatInfo provider1 = new NumberFormatInfo();
                        provider1.NumberDecimalSeparator = ",";
                        provider1.NumberGroupSeparator = "_";
                        provider1.NumberGroupSizes = new int[] { 3 };
                        NumberFormatInfo provider2 = new NumberFormatInfo();
                        provider2.NumberDecimalSeparator = ".";
                        provider2.NumberGroupSeparator = "_";
                        provider2.NumberGroupSizes = new int[] { 3 };

                        try { cutOff = Double.Parse(patient.CutOff, provider1); } catch { cutOff = Double.Parse(patient.CutOff, provider2); }
                        try { res = Double.Parse(patient.Result, provider1); } catch { res = Double.Parse(patient.Result, provider2); }
                        blotKP = res / cutOff;

                        TblDistrictBlot tblDistrictBlot = new()
                        {
                            PatientId = Int32.Parse(patient.PatienId),
                            DBlot = DateOnly.Parse(patient.Blotdate),
                            CutOff = cutOff,
                            BlotResult = res,
                            BlotCoefficient = blotKP,
                            TestSystemId = _context.ListTestSystems.First(e => e.TestSystemName == patient.TestSys).TestSystemId,
                            SendDistrictId = districtId,
                            SendLabId = labId
                        };

                        _context.TblDistrictBlots.Add(tblDistrictBlot);

                        TblIncomingBlood tblIncomingBlood = new()
                        {
                            PatientId = Int32.Parse(patient.PatienId),
                            SendDistrictId = districtId,
                            SendLabId = labId,
                            CategoryPatientId = Int32.Parse(patient.Category),
                            //AnonymousPatient = Boolean.Parse(patient.Anon),
                            DateBloodSampling = DateOnly.Parse(patient.DateBloodSampling),
                            DateBloodImport = DateOnly.FromDateTime(DateTime.Today),
                            NumIfa = (int)patient.NumIfa,
                            NumInList = Int32.Parse(patient.NumInList),
                            QualitySerumId = 11,
                            Repeat = null
                        };
                        _context.TblIncomingBloods.Add(tblIncomingBlood);

                        await _context.SaveChangesAsync();
                        successfulList.Add(new Success { NumIfa = (int)patient.NumIfa, Com = $"ИД: {patient.PatienId}; ФИО: {patient.FamilyName} {patient.FirstName} {patient.ThirdName};" });
                        //successfulList.Add($"ИД: {patient.PatienId}; Рег. ном.: {patient.NumIfa}; ФИО: {patient.FamilyName} {patient.FirstName} {patient.ThirdName};");
                   } 
                    catch { errList.Add($"ФИО: {patient.FamilyName} {patient.FirstName} {patient.ThirdName}; Дата рождения {patient.BirthDate}"); }
                }
                else 
                {
                    try 
                    {
                        TblPatientCard tblPatientCard;
                        int patientId = _context.TblPatientCards.Max(e => e.PatientId) + 1;

                        tblPatientCard = new()
                        {
                            PatientId = patientId,
                            FamilyName = patient.FamilyName,
                            FirstName = patient.FirstName,
                            ThirdName = patient.ThirdName,
                            BirthDate = DateOnly.Parse(patient.BirthDate),
                            SexId = _context.ListSexes.First(e => e.SexNameShort == patient.Sex).SexId,
                            RegionId = regionId,
                            CityName = patient.CityName,
                            AreaName = patient.AreaName,
                            PhoneNum = patient.Phone,
                            AddrHome = patient.AddrHome,
                            AddrCorps = patient.AddrCorps,
                            AddrFlat = patient.AddrFlat,
                            AddrStreat = patient.AddrStreat,
                            PatientCom = patient.PatientCom
                        };

                        _context.TblPatientCards.Add(tblPatientCard);
                        await _context.SaveChangesAsync();

                        TblIncomingBlood tblIncomingBlood = new()
                        {
                            PatientId = patientId,
                            SendDistrictId = districtId,
                            SendLabId = labId,
                            CategoryPatientId = Int32.Parse(patient.Category),
                            //AnonymousPatient = Boolean.Parse(patient.Anon),
                            DateBloodSampling = DateOnly.Parse(patient.DateBloodSampling),
                            DateBloodImport = DateOnly.FromDateTime(DateTime.Today),
                            NumIfa = (int)patient.NumIfa,
                            NumInList = Int32.Parse(patient.NumInList)
                        };

                        double cutOff, res, blotKP;
                        NumberFormatInfo provider1 = new NumberFormatInfo();
                        provider1.NumberDecimalSeparator = ",";
                        provider1.NumberGroupSeparator = "_";
                        provider1.NumberGroupSizes = new int[] { 3 };
                        NumberFormatInfo provider2 = new NumberFormatInfo();
                        provider2.NumberDecimalSeparator = ".";
                        provider2.NumberGroupSeparator = "_";
                        provider2.NumberGroupSizes = new int[] { 3 };

                        try { cutOff = Double.Parse(patient.CutOff, provider1); } catch { cutOff = Double.Parse(patient.CutOff, provider2); }
                        try { res = Double.Parse(patient.Result, provider1); } catch { res = Double.Parse(patient.Result, provider2); }
                        blotKP = res / cutOff;

                        TblDistrictBlot tblDistrictBlot = new()
                        {
                            PatientId = patientId,
                            DBlot = DateOnly.Parse(patient.Blotdate),
                            CutOff = cutOff,
                            BlotResult = res,
                            BlotCoefficient = blotKP,
                            TestSystemId = _context.ListTestSystems.First(e => e.TestSystemName == patient.TestSys).TestSystemId,
                            SendDistrictId = districtId,
                            SendLabId = labId
                        };

                        _context.TblIncomingBloods.Add(tblIncomingBlood);
                        _context.TblDistrictBlots.Add(tblDistrictBlot);
                        await _context.SaveChangesAsync();
                        successfulList.Add(new Success { NumIfa = (int)patient.NumIfa, Com = $"ИД: {patientId}; ФИО: {patient.FamilyName} {patient.FirstName} {patient.ThirdName};" });
                        //successfulList.Add($"ИД: {patient.PatienId}; Рег. ном.: {patient.NumIfa}; ФИО: {patient.FamilyName} {patient.FirstName} {patient.ThirdName};");
                    } 
                    catch { errList.Add($"ФИО: {patient.FamilyName} {patient.FirstName} {patient.ThirdName}; Дата рождения {patient.BirthDate}"); }
                }
            }
            ListForResultImport viewModel = new() 
            {
                Error = errList,
                SuccessList = successfulList.OrderBy(e => e.NumIfa).ToList()
            };

            ViewBag.Title = "ResultImport";
            return View("ResultImport", viewModel);
        }

        public static ListForImportPatient GetInputPatients(string path, Reference_AIDSContext _context)
        {
            ListForImportPatient listForImportPatient = new ListForImportPatient();
            List<InputPatients> list = new();

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(path, false))
            {
                IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                var rows = sheetData.Elements<Row>();
                int i = 0;

                //костыль переделать
                WorkbookPart wbPart = spreadsheetDocument.WorkbookPart;
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
                
                //wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "A" + row.RowIndex).InnerText)).InnerText

                foreach (var row in rows)
                {
                    if (row.RowIndex == 1)
                        continue;
                    string familyName = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "E" + row.RowIndex).InnerText)).InnerText;
                    List<TblPatientCard> posPatient = new();

                    if (familyName != "")
                        posPatient = _context.TblPatientCards.Where(e => e.FamilyName.ToUpper().Contains(familyName.ToUpper()))
                            .OrderBy(e => e.FamilyName).ThenBy(e => e.FirstName).ThenBy(e => e.ThirdName).ToList();


                    InputPatients patients = new();
                    patients.SendLab = GetCellValue(path, "A" + row.RowIndex);
                    patients.SendDistrict = GetCellValue(path, "B" + row.RowIndex);
                    patients.DateBloodSampling = DateTime.FromOADate(int.Parse(GetCellValue(path, "C" + row.RowIndex))).ToString("yyyy-MM-dd");
                    patients.Category = GetCellValue(path, "D" + row.RowIndex);
                    patients.FamilyName = GetCellValue(path, "E" + row.RowIndex);
                    patients.FirstName = GetCellValue(path, "F" + row.RowIndex);
                    patients.ThirdName = GetCellValue(path, "G" + row.RowIndex);
                    patients.BirthDate = DateTime.FromOADate(int.Parse(GetCellValue(path, "H" + row.RowIndex))).ToString("yyyy-MM-dd");
                    patients.Sex = GetCellValue(path, "I" + row.RowIndex);
                    patients.Phone = GetCellValue(path, "J" + row.RowIndex);
                    patients.RegionName = GetCellValue(path, "K" + row.RowIndex);
                    patients.CityName = GetCellValue(path, "L" + row.RowIndex);
                    patients.AreaName = GetCellValue(path, "M" + row.RowIndex);
                    patients.AddrStreat = GetCellValue(path, "N" + row.RowIndex);
                    patients.AddrHome = GetCellValue(path, "O" + row.RowIndex);
                    patients.AddrCorps = GetCellValue(path, "P" + row.RowIndex);
                    patients.AddrFlat = GetCellValue(path, "Q" + row.RowIndex);
                    patients.Blotdate = DateTime.FromOADate(int.Parse(GetCellValue(path, "R" + row.RowIndex))).ToString("yyyy-MM-dd");
                    patients.TestSys = GetCellValue(path, "S" + row.RowIndex);
                    patients.CutOff = GetCellValue(path, "T" + row.RowIndex);
                    patients.Result = GetCellValue(path, "U" + row.RowIndex);
                    patients.NumInList = GetCellValue(path, "V" + row.RowIndex);
                    patients.PossiblePatients = posPatient;
                    patients.NumPatient = i;

                    i++;
                    list.Add(patients);
                }
            }
            listForImportPatient.ListRegions = _context.ListRegions.ToList();
            listForImportPatient.ListSexes = _context.ListSexes.ToList();
            listForImportPatient.Patients = list.OrderBy(e => e.NumInList);
            listForImportPatient.ListSendLabs = _context.ListSendLabs.ToList();
            listForImportPatient.ListTestSystems = _context.ListTestSystems.ToList();
            listForImportPatient.ListSendDistricts = _context.ListSendDistricts.ToList();
            listForImportPatient.ListCategories = _context.ListCategories.ToList();
            return listForImportPatient;
        }

        static string GetCellValue(string fileName, string addressName)
        {
            string? value = null;
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().First();
                WorksheetPart wsPart = (WorksheetPart)wbPart.GetPartById(theSheet.Id!);

                if (addressName.Substring(0, 1) == "C" || addressName.Substring(0, 1) == "H" || addressName.Substring(0, 1) == "R")
                {
                    Worksheet worksheet = wsPart.Worksheet;
                    SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                    var rows = sheetData.Elements<Row>();
                    var row = rows.ElementAt(int.Parse(addressName.Substring(1))-1);
                    value = row.Elements<Cell>().First(c => c.CellReference == addressName).InnerText;
                    var a = int.Parse(value);
                    return value;
                }


                var sharedStringTable = wbPart.GetPartsOfType<SharedStringTablePart>().First().SharedStringTable;
                if (sharedStringTable is null)
                    return null;

                int ind;
                var b = wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == addressName).InnerText;

                if (!int.TryParse(b, out ind))
                    return null;

                if (addressName.Substring(0, 1) == "V")
                    return ind.ToString();

                value = sharedStringTable.ElementAt(ind).InnerText;


                return value;
            }
        }
    }
}
