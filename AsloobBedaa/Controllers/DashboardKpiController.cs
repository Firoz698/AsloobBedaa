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

        public async Task<IActionResult> Index()
        {
            var data = await _context.DashboardKpis
                                     .OrderBy(x => x.Id)
                                     .ToListAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DashboardKpi model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedDate = DateTime.Now;
            model.Status = CalculateStatus(model.CurrentMonth, model.PreviousMonth);

            _context.DashboardKpis.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var kpi = await _context.DashboardKpis.FindAsync(id);
            if (kpi == null)
                return NotFound();

            return View(kpi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DashboardKpi model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.DashboardKpis.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.KpiName = model.KpiName;
            existing.CurrentMonth = model.CurrentMonth;
            existing.PreviousMonth = model.PreviousMonth;
            existing.YearToDate = model.YearToDate;
            existing.Remarks = model.Remarks;

            // Recalculate status safely
            existing.Status = CalculateStatus(model.CurrentMonth, model.PreviousMonth);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var kpi = await _context.DashboardKpis
                                    .FirstOrDefaultAsync(x => x.Id == id);
            if (kpi == null)
                return NotFound();

            return View(kpi);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var kpi = await _context.DashboardKpis.FindAsync(id);
            if (kpi == null)
                return NotFound();

            return View(kpi);
        }

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

        private string CalculateStatus(decimal current, decimal previous)
        {
            if (current > previous) return "Increase";
            if (current < previous) return "Decrease";
            return "Stable";
        }
    }

}
