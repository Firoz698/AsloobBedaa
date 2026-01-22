using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsloobBedaa.Controllers
{
    public class SubcontractorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubcontractorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subcontractor
        public async Task<IActionResult> Index()
        {
            var data = await _context.Subcontractors
                                     .OrderByDescending(x => x.SubcontractorId)
                                     .ToListAsync();
            return View(data);
        }

        // GET: Subcontractor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subcontractor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subcontractor model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;

                // Auto Outstanding Balance calculation
                model.OutstandingBalance =
                    model.ContractValue - model.PaidAmount;

                _context.Subcontractors.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Subcontractor/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var subcontractor = await _context.Subcontractors.FindAsync(id);
            if (subcontractor == null)
                return NotFound();

            return View(subcontractor);
        }

        // POST: Subcontractor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subcontractor model)
        {
            if (id != model.SubcontractorId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Subcontractors.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.SubcontractorName = model.SubcontractorName;
                existing.ProjectName = model.ProjectName;
                existing.ContractValue = model.ContractValue;
                existing.MonthlyBilling = model.MonthlyBilling;
                existing.PaidAmount = model.PaidAmount;
                existing.RetentionAmount = model.RetentionAmount;
                existing.Status = model.Status;
                existing.Remarks = model.Remarks;

                // Recalculate Outstanding Balance
                existing.OutstandingBalance =
                    model.ContractValue - model.PaidAmount;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Subcontractor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var subcontractor = await _context.Subcontractors
                                              .FirstOrDefaultAsync(x => x.SubcontractorId == id);
            if (subcontractor == null)
                return NotFound();

            return View(subcontractor);
        }

        // GET: Subcontractor/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var subcontractor = await _context.Subcontractors.FindAsync(id);
            if (subcontractor == null)
                return NotFound();

            return View(subcontractor);
        }

        // POST: Subcontractor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcontractor = await _context.Subcontractors.FindAsync(id);
            if (subcontractor != null)
            {
                _context.Subcontractors.Remove(subcontractor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
