using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using Reference_Aids.ModelsForInput;
using Reference_Aids.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Reference_Aids.ViewModels;
using System.Drawing;
using System.Linq;

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
                //path = @"C:\Users\alexk\Source\Repos\Alex-Krasnov\Reference_Aids_git\wwwroot\Files\ForInput\ОтправкаВГЛ_экспорт.xls";
                dataList = GetInputPatients(path, _context);
            }
            ViewBag.Title = "InputPatient";
            
            return View("RawImport", dataList);
        }

        [HttpPost]
        public async Task<IActionResult> Input(List<InputPatients> listPatients)
        {
            foreach (var patient in listPatients)
            {
                int labId = _context.ListSendLabs.First(e => e.SendLabName == patient.SendLab).SendLabId,
                    districtId = _context.ListSendDistricts.First(e => e.SendDistrictName == patient.SendDistrict).SendDistrictId;

                if (patient.PatienId != null)
                {
                    TblDistrictBlot tblDistrictBlot = new()
                    {
                        PatientId = Int32.Parse(patient.PatienId),
                        DBlot = DateOnly.Parse(patient.Blotdate),
                        CutOff =  Double.Parse(patient.CutOff),
                        BlotResult = Double.Parse(patient.Result),
                        BlotCoefficient = (Double.Parse(patient.Result) / Double.Parse(patient.CutOff)),
                        TestSystemId = _context.ListTestSystems.First(e => e.TestSystemName == patient.TestSys).TestSystemId,
                        SendDistrictId = districtId,
                        SendLabId = labId
                    };
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
                        NumInList = Int32.Parse(patient.NumInList)
                    };

                    _context.TblIncomingBloods.Add(tblIncomingBlood);
                    _context.TblDistrictBlots.Add(tblDistrictBlot);
                    await _context.SaveChangesAsync();
                }
                else 
                {
                    TblPatientCard tblPatientCard;
                    try
                    {
                        tblPatientCard = new()
                        {
                            FamilyName = patient.FamilyName,
                            FirstName = patient.FirstName,
                            ThirdName = patient.ThirdName,
                            BirthDate = DateOnly.Parse(patient.BirthDate),
                            SexId = _context.ListSexes.First(e => e.SexNameShort == patient.Sex).SexId,
                            RegionId = _context.ListRegions.First(e => e.RegionName == patient.RegionName).RegionId,
                            CityName = patient.CityName,
                            AreaName = patient.AreaName,
                            PhoneNum = patient.Phone,
                            AddrHome = patient.AddrHome,
                            AddrCorps = patient.AddrCorps,
                            AddrFlat = patient.AddrFlat,
                            AddrStreat = patient.AddrStreat
                        };
                    }
                    catch
                    {
                        tblPatientCard = new()
                        {
                            BirthDate = DateOnly.Parse(patient.BirthDate),
                            SexId = _context.ListSexes.First(e => e.SexNameShort == patient.Sex).SexId,
                            RegionId = _context.ListRegions.First(e => e.RegionName == patient.RegionName).RegionId,
                            CityName = patient.CityName,
                            AreaName = patient.AreaName,
                            PhoneNum = patient.Phone,
                            AddrHome = patient.AddrHome,
                            AddrCorps = patient.AddrCorps,
                            AddrFlat = patient.AddrFlat,
                            AddrStreat = patient.AddrStreat
                        };
                    }
                    
                    _context.TblPatientCards.Add(tblPatientCard);
                    await _context.SaveChangesAsync();
                    int patienId = _context.TblPatientCards.First(e => e.FirstName == patient.FirstName &&
                                                                       e.FamilyName == patient.FamilyName &&
                                                                       e.ThirdName == patient.ThirdName &&
                                                                       e.BirthDate == DateOnly.Parse(patient.BirthDate) &&
                                                                       e.CityName == patient.CityName &&
                                                                       e.AreaName == patient.AreaName).PatientId;

                    TblIncomingBlood tblIncomingBlood = new()
                    {
                        PatientId = patienId,
                        SendDistrictId = districtId,
                        SendLabId = labId,
                        CategoryPatientId = Int32.Parse(patient.Category),
                        //AnonymousPatient = Boolean.Parse(patient.Anon),
                        DateBloodSampling = DateOnly.Parse(patient.DateBloodSampling),
                        DateBloodImport = DateOnly.FromDateTime(DateTime.Today),
                        NumIfa = (int)patient.NumIfa,
                        NumInList = Int32.Parse(patient.NumInList)
                    };
                    TblDistrictBlot tblDistrictBlot = new()
                    {
                        PatientId = patienId,
                        DBlot = DateOnly.Parse(patient.Blotdate),
                        CutOff = Double.Parse(patient.CutOff),
                        BlotResult = Double.Parse(patient.Result),
                        BlotCoefficient = (Double.Parse(patient.Result) / Double.Parse(patient.CutOff)),
                        TestSystemId = _context.ListTestSystems.First(e => e.TestSystemName == patient.TestSys).TestSystemId,
                        SendDistrictId = districtId,
                        SendLabId = labId
                    };

                    _context.TblIncomingBloods.Add(tblIncomingBlood);
                    _context.TblDistrictBlots.Add(tblDistrictBlot);
                    await _context.SaveChangesAsync();
                }
            }
            ViewBag.Title = "InputPatientSuccess";
            return View("Index");
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
                        posPatient = _context.TblPatientCards.Where(e => e.FamilyName.ToUpper().Contains(familyName.ToUpper())).OrderBy(e => e.PatientId).ToList();


                    InputPatients patients = new()
                    {
                        SendLab = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "A" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "A"+ row.RowIndex).InnerText,
                        SendDistrict = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "B" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "B" + row.RowIndex).InnerText,
                        DateBloodSampling = row.Elements<Cell>().First(c => c.CellReference == "C" + row.RowIndex).InnerText,//DateTime.FromOADate(int.Parse(row.Elements<Cell>().First(c => c.CellReference == "C" + row.RowIndex).InnerText)).ToString("yyyy-MM-dd")
                        Category = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "D" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "D" + row.RowIndex).InnerText,
                        //Anon = row.Elements<Cell>().First(c => c.CellReference == "E" + row.RowIndex).InnerText,
                        FamilyName = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "E" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "E" + row.RowIndex).InnerText,
                        FirstName = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "F" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "F" + row.RowIndex).InnerText,
                        ThirdName = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "G" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "G" + row.RowIndex).InnerText,
                        BirthDate = row.Elements<Cell>().First(c => c.CellReference == "H" + row.RowIndex).InnerText,//DateTime.FromOADate(int.Parse(row.Elements<Cell>().First(c => c.CellReference == "H" + row.RowIndex).InnerText)).ToString("yyyy-MM-dd")
                        Sex = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "I" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "I" + row.RowIndex).InnerText,
                        Phone = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "J" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "J" + row.RowIndex).InnerText,
                        RegionName = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "K" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "K" + row.RowIndex).InnerText,
                        CityName = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "L" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "L" + row.RowIndex).InnerText,
                        AreaName = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "M" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "M" + row.RowIndex).InnerText,
                        AddrStreat = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "N" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "N" + row.RowIndex).InnerText,
                        AddrHome = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "O" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "O" + row.RowIndex).InnerText,
                        AddrCorps = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "P" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "P" + row.RowIndex).InnerText,
                        AddrFlat = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "Q" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "Q" + row.RowIndex).InnerText,
                        Blotdate = row.Elements<Cell>().First(c => c.CellReference == "R" + row.RowIndex).InnerText,//DateTime.FromOADate(int.Parse(row.Elements<Cell>().First(c => c.CellReference == "R" + row.RowIndex).InnerText)).ToString("yyyy-MM-dd")
                        TestSys = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "S" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "S" + row.RowIndex).InnerText,
                        CutOff = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "T" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "T" + row.RowIndex).InnerText,
                        Result = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "U" + row.RowIndex).InnerText)).InnerText,//row.Elements<Cell>().First(c => c.CellReference == "U" + row.RowIndex).InnerText,
                        NumInList = row.Elements<Cell>().First(c => c.CellReference == "V" + row.RowIndex).InnerText,//wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(wsPart.Worksheet.Descendants<Cell>().First(c => c.CellReference == "V" + row.RowIndex).InnerText)).InnerText,
                        PossiblePatients = posPatient,
                        NumPatient = i
                    };
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
    }
}
