using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LaboratoryISCore.Data;
using LaboratoryISCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace LaboratoryISCore.Controllers
{
    [Authorize]
    public class AnalysesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalysesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Analyses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Analysis.Include(a => a.AnalysisGroup);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Analyses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysis = await _context.Analysis
                .Include(a => a.AnalysisGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (analysis == null)
            {
                return NotFound();
            }

            return View(analysis);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult Check(string AnalysisName,int Id)
        {
            if (Id== 0) 
            {
                var a = _context.Analysis.Where(m => m.AnalysisName.ToUpper() == AnalysisName.ToUpper()).Count();
                if (a != 0)
                    return Json(false);
                return Json(true);
            }
            else
            {
                return Json(true);
            }
        }
       
        // GET: Analyses/Create
        public IActionResult Create()
        {
            Analysis analysis = new Analysis();
            Parameter parameter = new Parameter();
            analysis.Parameters.Add(parameter);
            ViewData["AnalysisGroupId"] = new SelectList(_context.AnalysisGroups, "Id", "AnalysisGroupName");
            ViewData["ParameterTypeId"] = new SelectList(_context.ParameterTypes, "Id", "ParameterTypeName");
            ViewData["TemplateId"] = new SelectList(_context.Templates, "Id", "TemplateName");
            return View(analysis);
        }

        // POST: Analyses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Analysis analysis)
        {
            if (ModelState.IsValid)
            {
                List<Parameter> parameters = new List<Parameter>();
                parameters = analysis.Parameters;
                Analysis model = analysis.Copy();
                _context.Add(model);
                await _context.SaveChangesAsync();
                foreach (var temp in parameters)
                {
                    temp.AnalysisId = model.Id;
                    _context.Add(temp);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnalysisGroupId"] = new SelectList(_context.AnalysisGroups, "Id", "AnalysisGroupName", analysis.AnalysisGroupId);
            ViewData["ParameterTypeId"] = new SelectList(_context.ParameterTypes, "Id", "ParameterTypeName");
            ViewData["TemplateId"] = new SelectList(_context.Templates, "Id", "TemplateName");
            return View(analysis);
        }

        // GET: Analyses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            Parameter parameter = new Parameter();
            if (id == null)
            {
                return NotFound();
            }

            var analysis = await _context.Analysis.FindAsync(id);
            if (analysis == null)
            {
                return NotFound();
            }
            ViewData["AnalysisGroupId"] = new SelectList(_context.AnalysisGroups, "Id", "AnalysisGroupName", analysis.AnalysisGroupId);
            ViewData["ParameterTypeId"] = new SelectList(_context.ParameterTypes, "Id", "ParameterTypeName");
            ViewData["TemplateId"] = new SelectList(_context.Templates, "Id", "TemplateName");
            ViewData["VisibilityId"] = new SelectList(_context.Visibilities, "Id", "VisibilityName");
            var parameters = _context.Parameters.Where(e => e.AnalysisId == id).ToList();
            return View(analysis);
        }

        // POST: Analyses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Analysis analysis)

        {
            List<Parameter> parameters = new List<Parameter>();
            parameters = analysis.Parameters;
          //  Analysis model = analysis.Copy();

            if (id != analysis.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(analysis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnalysisExists(analysis.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnalysisGroupId"] = new SelectList(_context.AnalysisGroups, "Id", "AnalysisGroupName", analysis.AnalysisGroupId);
            ViewData["ParameterTypeId"] = new SelectList(_context.ParameterTypes, "Id", "ParameterTypeName");
            ViewData["TemplateId"] = new SelectList(_context.Templates, "Id", "TemplateName");
            ViewData["VisibilityId"] = new SelectList(_context.Visibilities, "Id", "VisibilityName");
            return View(analysis);
        }

        // GET: Analyses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analysis = await _context.Analysis
                .Include(a => a.AnalysisGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (analysis == null)
            {
                return NotFound();
            }

            return View(analysis);
        }

        // POST: Analyses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var analysis = await _context.Analysis.FindAsync(id);
            _context.Analysis.Remove(analysis);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnalysisExists(int id)
        {
            return _context.Analysis.Any(e => e.Id == id);
        }
    }
}
