using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsloobBedaa.Controllers
{
    public class AccountsTransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountsTransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AccountsTransaction
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.AccountsTransactions
                                             .OrderByDescending(x => x.InvoiceDate)
                                             .ToListAsync();
            return View(transactions);
        }

        // GET: AccountsTransaction/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountsTransaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountsTransaction model)
        {
            if (ModelState.IsValid)
            {
                // Calculate Balance
                model.BalanceAmount = model.InvoiceAmount - model.PaidAmount;

                // Calculate AgingDays
                model.AgingDays = (DateTime.Now - model.DueDate).Days;

                // Calculate Status
                if (model.BalanceAmount <= 0)
                    model.Status = "Paid";
                else if (model.BalanceAmount > 0 && model.DueDate >= DateTime.Now)
                    model.Status = "Partial";
                else
                    model.Status = "Overdue";

                _context.AccountsTransactions.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: AccountsTransaction/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _context.AccountsTransactions.FindAsync(id);
            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        // POST: AccountsTransaction/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AccountsTransaction model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.AccountsTransactions.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.Type = model.Type;
                existing.ClientOrVendorName = model.ClientOrVendorName;
                existing.InvoiceNo = model.InvoiceNo;
                existing.InvoiceDate = model.InvoiceDate;
                existing.InvoiceAmount = model.InvoiceAmount;
                existing.PaidAmount = model.PaidAmount;
                existing.DueDate = model.DueDate;
                existing.Remarks = model.Remarks;

                // Recalculate Balance
                existing.BalanceAmount = model.InvoiceAmount - model.PaidAmount;

                // Recalculate AgingDays
                existing.AgingDays = (DateTime.Now - model.DueDate).Days;

                // Recalculate Status
                if (existing.BalanceAmount <= 0)
                    existing.Status = "Paid";
                else if (existing.BalanceAmount > 0 && existing.DueDate >= DateTime.Now)
                    existing.Status = "Partial";
                else
                    existing.Status = "Overdue";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: AccountsTransaction/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _context.AccountsTransactions
                                            .FirstOrDefaultAsync(x => x.Id == id);
            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        // GET: AccountsTransaction/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.AccountsTransactions.FindAsync(id);
            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        // POST: AccountsTransaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.AccountsTransactions.FindAsync(id);
            if (transaction != null)
            {
                _context.AccountsTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
