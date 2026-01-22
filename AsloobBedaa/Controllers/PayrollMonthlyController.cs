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
            var payrolls = await _context.PayrollMonthlies
                                         .OrderByDescending(x => x.Month)
                                         .ToListAsync();
            return View(payrolls);
        }

        // GET: PayrollMonthly/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PayrollMonthly/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollMonthly model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;

                // Auto-calculate NetPayrollCost
                model.NetPayrollCost = model.BasicSalary + model.Overtime + model.Allowances - model.Deductions;

                _context.PayrollMonthlies.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: PayrollMonthly/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            if (payroll == null)
                return NotFound();

            return View(payroll);
        }

        // POST: PayrollMonthly/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayrollMonthly model)
        {
            if (id != model.PayrollId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.PayrollMonthlies.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.Month = model.Month;
                existing.ProjectName = model.ProjectName;
                existing.NumberOfEmployees = model.NumberOfEmployees;
                existing.BasicSalary = model.BasicSalary;
                existing.Overtime = model.Overtime;
                existing.Allowances = model.Allowances;
                existing.Deductions = model.Deductions;
                existing.WpsStatus = model.WpsStatus;
                existing.Remarks = model.Remarks;

                // Recalculate NetPayrollCost
                existing.NetPayrollCost = model.BasicSalary + model.Overtime + model.Allowances - model.Deductions;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: PayrollMonthly/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var payroll = await _context.PayrollMonthlies
                                        .FirstOrDefaultAsync(x => x.PayrollId == id);
            if (payroll == null)
                return NotFound();

            return View(payroll);
        }

        // GET: PayrollMonthly/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            if (payroll == null)
                return NotFound();

            return View(payroll);
        }

        // POST: PayrollMonthly/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            if (payroll != null)
            {
                _context.PayrollMonthlies.Remove(payroll);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
