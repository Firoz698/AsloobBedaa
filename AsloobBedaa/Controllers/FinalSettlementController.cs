using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsloobBedaa.Controllers
{
    public class FinalSettlementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinalSettlementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FinalSettlement
        public async Task<IActionResult> Index()
        {
            var settlements = await _context.FinalSettlements
                                            .OrderByDescending(x => x.LastWorkingDay)
                                            .ToListAsync();
            return View(settlements);
        }

        // GET: FinalSettlement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FinalSettlement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FinalSettlement model)
        {
            if (ModelState.IsValid)
            {
                // Calculate NetSettlementAmount
                model.NetSettlementAmount = model.PendingSalary + model.LeaveSalary + model.Gratuity - model.Deductions;

                _context.FinalSettlements.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: FinalSettlement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var settlement = await _context.FinalSettlements.FindAsync(id);
            if (settlement == null)
                return NotFound();

            return View(settlement);
        }

        // POST: FinalSettlement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FinalSettlement model)
        {
            if (id != model.SettlementId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.FinalSettlements.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.EmployeeName = model.EmployeeName;
                existing.EmployeeId = model.EmployeeId;
                existing.ProjectName = model.ProjectName;
                existing.LastWorkingDay = model.LastWorkingDay;
                existing.PendingSalary = model.PendingSalary;
                existing.LeaveSalary = model.LeaveSalary;
                existing.Gratuity = model.Gratuity;
                existing.Deductions = model.Deductions;
                existing.PaymentStatus = model.PaymentStatus;
                existing.Remarks = model.Remarks;

                // Recalculate NetSettlementAmount
                existing.NetSettlementAmount = model.PendingSalary + model.LeaveSalary + model.Gratuity - model.Deductions;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: FinalSettlement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var settlement = await _context.FinalSettlements
                                           .FirstOrDefaultAsync(x => x.SettlementId == id);
            if (settlement == null)
                return NotFound();

            return View(settlement);
        }

        // GET: FinalSettlement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var settlement = await _context.FinalSettlements.FindAsync(id);
            if (settlement == null)
                return NotFound();

            return View(settlement);
        }

        // POST: FinalSettlement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var settlement = await _context.FinalSettlements.FindAsync(id);
            if (settlement != null)
            {
                _context.FinalSettlements.Remove(settlement);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
