﻿using Microsoft.AspNetCore.Mvc;
using Reference_Aids.Data;
using Reference_Aids.Models;
using Reference_Aids.ModelsForInput;
using Reference_Aids.ViewModels;

namespace Reference_Aids.Controllers
{
    public class RegPatientCardsController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public RegPatientCardsController(Reference_AIDSContext context)
        {
            _context=context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Viewdata = new ListForRegViewModel
            {
                NextPatientID = _context.TblPatientCards.Max(b => b.PatientId) + 1,
                ListSexes = _context.ListSexes.ToList(),
                ListRegions = _context.ListRegions.ToList(),
                ListSendLabs = _context.ListSendLabs.ToList(),
                ListTestSystems = _context.ListTestSystems.ToList(),
                ListSendDistricts = _context.ListSendDistricts.ToList()
            };
            ViewBag.Title = "RegPatient";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InputRegPatient list)
        {
            if (ModelState.IsValid)
            {
                TblPatientCard tblPatientCard = new()
                {
                    PatientId = list.PatientId,
                    FamilyName = list.FamilyName,
                    FirstName = list.FirstName,
                    ThirdName = list.ThirdName,
                    BirthDate = DateOnly.Parse(list.BirthDate),
                    SexId = list.SexId(_context),
                    RegionId = list.RegionId(_context),
                    CityName = list.CityName,
                    AreaName = list.AreaName,
                    PatientCom = list.PatientCom,
                    PhoneNum = list.PhoneNum,
                    AddrHome = list.AddrHome,
                    AddrCorps = list.AddrCorps,
                    AddrFlat = list.AddrFlat,
                    AddrStreat = list.AddrStreat,
                    Snils = list.Snils
                };

                TblDistrictBlot tblDistrictBlot = new()
                {
                    PatientId = list.PatientId,
                    DBlot = DateOnly.Parse(list.DBlot),
                    CutOff = list.CutOff,
                    BlotResult = list.BlotResult,
                    BlotCoefficient = (list.BlotResult / list.CutOff),
                    TestSystemId = list.TestSystemId(_context),
                    SendDistrictId = list.SendDistrictId(_context),
                    SendLabId = list.SendLabId(_context)
                };

                _context.TblPatientCards.Add(tblPatientCard);
                _context.TblDistrictBlots.Add(tblDistrictBlot);
                _context.SaveChanges();
                return RedirectToAction("Index", "RegIncBloods", new { id = tblPatientCard.PatientId});
            }
            return RedirectToAction("Index");
        }
    }
}
