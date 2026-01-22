using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsloobBedaa.Controllers
{
    public class DashboardKpiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardKpiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DashboardKpi
        public async Task<IActionResult> Index()
        {
            var data = await _context.DashboardKpis
                                     .OrderBy(x => x.Id)
                                     .ToListAsync();
            return View(data);
        }

        // GET: DashboardKpi/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DashboardKpi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DashboardKpi model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;

                // Status auto calculation
                if (model.CurrentMonth > model.PreviousMonth)
                    model.Status = "Increase";
                else if (model.CurrentMonth < model.PreviousMonth)
                    model.Status = "Decrease";
                else
                    model.Status = "Stable";

                _context.DashboardKpis.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: DashboardKpi/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var kpi = await _context.DashboardKpis.FindAsync(id);
            if (kpi == null)
                return NotFound();

            return View(kpi);
        }

        // POST: DashboardKpi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DashboardKpi model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.DashboardKpis.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.KpiName = model.KpiName;
                existing.CurrentMonth = model.CurrentMonth;
                existing.PreviousMonth = model.PreviousMonth;
                existing.YearToDate = model.YearToDate;
                existing.Remarks = model.Remarks;

                // Recalculate Status
                if (model.CurrentMonth > model.PreviousMonth)
                    existing.Status = "Increase";
                else if (model.CurrentMonth < model.PreviousMonth)
                    existing.Status = "Decrease";
                else
                    existing.Status = "Stable";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: DashboardKpi/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var kpi = await _context.DashboardKpis.FirstOrDefaultAsync(x => x.Id == id);
            if (kpi == null)
                return NotFound();

            return View(kpi);
        }

        // GET: DashboardKpi/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var kpi = await _context.DashboardKpis.FindAsync(id);
            if (kpi == null)
                return NotFound();

            return View(kpi);
        }

        // POST: DashboardKpi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kpi = await _context.DashboardKpis.FindAsync(id);
            if (kpi != null)
            {
                _context.DashboardKpis.Remove(kpi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
