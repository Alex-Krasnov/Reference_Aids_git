﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class ImportAnalyzesController : Controller
    {
        private readonly Reference_AIDSContext _context;
        public ImportAnalyzesController(Reference_AIDSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<string> listTypeAnalyze = new() { "ИФА", "Антиген", "Подтв ag"};
            var Viewdata = new ListForImportAnalyzes
            {
                ListTestSystems = await _context.ListTestSystems.ToListAsync(),
                ListTypeAnalyzes = listTypeAnalyze
            };

            ViewBag.Title = "ImportAnalyzes";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public IActionResult RoutingForPhotometrAnalyzes(string _reportId, string _testSystem, string _typeAnalyzes)
        {
            switch (_typeAnalyzes){
                case "ИФА":
                    return RedirectToAction("Index", "ImportPhotometr", new { reportId = _reportId, testSystem = _testSystem, typeAnalyzes = _typeAnalyzes });
                case "Антиген":
                    return RedirectToAction("Index", "ImportPhotometr", new { reportId = _reportId, testSystem = _testSystem, typeAnalyzes = _typeAnalyzes });
                case "Подтв ag":
                    return RedirectToAction("Index", "ImportPhotometr", new { reportId = _reportId, testSystem = _testSystem, typeAnalyzes = _typeAnalyzes });
                default: 
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult RoutingForBlotAnalyzes(int _countRow, string _date, string _testSystem)
        {
            return RedirectToAction("Index", "ImportBlot", new { countRow = _countRow, testSystem = _testSystem, date = _date });
        }

        [HttpPost]
        public IActionResult RoutingForPcrAnalyzes(int _numIfaStart,int _numIfaEnd, string _testSystem)
        {
            return RedirectToAction("Index", "ImportPcr", new { numIfaStart = _numIfaStart, numIfaEnd= _numIfaEnd, testSystem = _testSystem});
        }
    }
}