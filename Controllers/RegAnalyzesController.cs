﻿using Microsoft.AspNetCore.Mvc;
using Reference_Aids.ViewModels;
using Reference_Aids.Models;
using Reference_Aids.Data;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.ModelsForInput;

namespace Reference_Aids.Controllers
{
    public class RegAnalyzesController : Controller
    {
        private readonly Reference_AIDSContext _context;

        public RegAnalyzesController(Reference_AIDSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int blood_id)
        {
            var Viewdata = new ListForInputAnalyzesViewModel
            {
                ListTestSystems = await _context.ListTestSystems.ToListAsync(),
                ListResults = await _context.ListResults.ToListAsync(),
                ListTypeAntigens = await _context.ListTypeAntigens.ToListAsync(),
                TblResultAntigens = await _context.TblResultAntigens.Where(e => e.BloodId == blood_id).ToListAsync(),
                TblResultPcrs = await _context.TblResultPcrs.Where(e => e.BloodId == blood_id).ToListAsync(),
                TblResultIfas = await _context.TblResultIfas.Where(e => e.BloodId == blood_id).ToListAsync(),
                TblResultBlots = await _context.TblResultBlots.Where(e => e.BloodId == blood_id).ToListAsync(),
                TblIncomingBloods = await _context.TblIncomingBloods.Where(e => e.BloodId == blood_id).ToListAsync()
            };

            ViewBag.Title = "RegAnalyzes";
            return View("Index", Viewdata);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnalyzesAntigen(InputAntigenResult list)
        {
            if (ModelState.IsValid)
            {
                int? percentGash = null;
                double? kp = null;

                if (list.TypeAntigen(_context) == 0)
                    kp = (list.ResultAntigenOp / list.ResultAntigenCutOff);
                else
                    percentGash = (int)((list.ResultAntigenOp - list.ResultAntigenConfirmOp) / list.ResultAntigenOp) * 100;   

                TblResultAntigen tblResultAntigen = new()
                {
                    BloodId = list.BloodId,
                    ResultAntigenDate = DateOnly.Parse(list.ResultAntigenDate),
                    ResultAntigenTestSysId = list.TestSysId(_context),
                    ResultAntigenCutOff = list.ResultAntigenCutOff,
                    ResultAntigenOp = list.ResultAntigenOp,
                    ResultAntigenConfirmOp = list.ResultAntigenConfirmOp,
                    ResultAntigenPercentGash = percentGash,
                    ResultAntigenKp = kp,
                    ResultAntigenTypeId = list.TypeAntigen(_context),
                    ResultAntigenResultId = 0 // положительный для обычного антигена если оп >= ОП крит.
                                              // положительный для подтв антигена если оп >= оп крит И percentGash >= 50%
                                              // ОП крит.(CutOff) находится в formulas: calculated : result    
                };

                _context.TblResultAntigens.Add(tblResultAntigen);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { blood_id = list.BloodId });
            }
            return RedirectToAction("Index", new { blood_id = list.BloodId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnalyzesPcr(InputPcr list)
        {
            if (ModelState.IsValid)
            {
                TblResultPcr tblResultPcr = new()
                {
                    BloodId = list.BloodId,
                    ResultPcrId = list.ResultPcrId,
                    ResultPcrDate = DateOnly.Parse(list.ResultPcrDate),
                    ResultPcrTestSysId = list.TestSysId(_context),
                    ResultPcrResultId = list.ResultId(_context)
                };

                _context.TblResultPcrs.Add(tblResultPcr);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { blood_id = list.BloodId });
            }
            return RedirectToAction("Index", new { blood_id = list.BloodId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnalyzesBlot(InputBlot list)
        {
            if (ModelState.IsValid)
            {
                TblResultBlot tblResultBlot = new()
                {
                    BloodId = list.BloodId,
                    ResultBlotDate = DateOnly.Parse(list.ResultBlotDate),
                    ExpirationResultBlotDate = DateOnly.Parse(list.ExpirationResultBlotDate),
                    ResultBlotTestSysId =list.TestSysId(_context),
                    ResultBlotEnv160 = list.ResultBlotEnv160Id(_context),
                    ResultBlotEnv120 = list.ResultBlotEnv120Id(_context),
                    ResultBlotEnv41 = list.ResultBlotEnv41Id(_context),
                    ResultBlotGag55 = list.ResultBlotGag55Id(_context),
                    ResultBlotGag40 = list.ResultBlotGag40Id(_context),
                    ResultBlotGag2425 = list.ResultBlotGag2425Id(_context),
                    ResultBlotGag18 = list.ResultBlotGag18Id(_context),
                    ResultBlotPol6866 = list.ResultBlotPol6866Id(_context),
                    ResultBlotPol5251 = list.ResultBlotPol5251Id(_context),
                    ResultBlotPol3431 = list.ResultBlotPol3431Id(_context),
                    ResultBlotHiv2105 = list.ResultBlotHiv2105Id(_context),
                    ResultBlotHiv236 = list.ResultBlotHiv236Id(_context),
                    ResultBlotHiv0 = list.ResultBlotHiv0Id(_context),
                    ResultBlotResultId = list.ResultId(_context)
                };

                _context.TblResultBlots.Add(tblResultBlot);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { blood_id = list.BloodId });
            }
            return RedirectToAction("Index", new { blood_id = list.BloodId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnalyzesIfa(InputIfa list)
        {
            if (ModelState.IsValid)
            {
                TblResultIfa tblResultIfa = new()
                {
                    BloodId = list.BloodId,
                    ResultIfaDate = DateOnly.Parse(list.ResultIfaDate),
                    ResultIfaTestSysId = list.TestSysId(_context),
                    ResultIfaCutOff = list.ResultIfaCutOff,
                    ResultIfaOp = list.ResultIfaOp,
                    ResultIfaResultId = 0 // положитнльный если оп >= cutoff и на оборот
                };

                _context.TblResultIfas.Add(tblResultIfa);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { blood_id = list.BloodId });
            }
            return RedirectToAction("Index", new { blood_id = list.BloodId });
        }
    }
}
