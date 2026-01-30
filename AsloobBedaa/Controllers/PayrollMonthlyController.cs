using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsloobBedaa.Controllers
{
    public class PayrollMonthlyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollMonthlyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PayrollMonthly
        public async Task<IActionResult> Index()
        {
            var payrolls = await _context.PayrollMonthlies.ToListAsync();
            return View(payrolls);
        }

        // GET: PayrollMonthly/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payroll = await _context.PayrollMonthlies.FirstOrDefaultAsync(p => p.PayrollId == id);
            if (payroll == null) return NotFound();

            return View(payroll);
        }

        // GET: PayrollMonthly/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PayrollMonthly/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollMonthly payroll)
        {
            if (ModelState.IsValid)
            {
                payroll.CreatedDate = DateTime.Now;
                _context.Add(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payroll);
        }

        // GET: PayrollMonthly/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            if (payroll == null) return NotFound();

            return View(payroll);
        }

        // POST: PayrollMonthly/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayrollMonthly payroll)
        {
            if (id != payroll.PayrollId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payroll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollMonthlyExists(payroll.PayrollId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payroll);
        }

        // GET: PayrollMonthly/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payroll = await _context.PayrollMonthlies.FirstOrDefaultAsync(p => p.PayrollId == id);
            if (payroll == null) return NotFound();

            return View(payroll);
        }

        // POST: PayrollMonthly/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            _context.PayrollMonthlies.Remove(payroll);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollMonthlyExists(int id)
        {
            return _context.PayrollMonthlies.Any(e => e.PayrollId == id);
        }
    }
}
