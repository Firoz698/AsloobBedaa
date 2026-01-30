using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using AsloobBedaa.Services;

namespace AsloobBedaa.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogger _actilogger;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            ActivityLogger actilogger)
        {
            _logger = logger;
            _context = context;
            _actilogger = actilogger;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            // KPI list
            var dashboardKpis = await _context.DashboardKpis
                                              .OrderBy(x => x.Id)
                                              .ToListAsync();

            // Active Projects
            ViewBag.ActiveProjects = await _context.Subcontractors
                .CountAsync(s => s.Status == "Active");

            // Current month range
            var now = DateTime.Now;
            var firstDay = new DateTime(now.Year, now.Month, 1);
            var lastDay = firstDay.AddMonths(1);

            // Monthly Payroll (SAFE & FAST)
            ViewBag.MonthlyPayroll = await _context.PayrollMonthlies
                .Where(p => p.Month >= firstDay && p.Month < lastDay)
                .SumAsync(p => (decimal?)p.NetPayrollCost) ?? 0;

            // Accounts Receivable
            ViewBag.AccountsReceivable = await _context.AccountsTransactions
                .Where(a => a.Type == "AR")
                .SumAsync(a => (decimal?)a.BalanceAmount) ?? 0;

            // Accounts Payable
            ViewBag.AccountsPayable = await _context.AccountsTransactions
                .Where(a => a.Type == "AP")
                .SumAsync(a => (decimal?)a.BalanceAmount) ?? 0;

            // Pending Final Settlements
            ViewBag.PendingSettlements = await _context.FinalSettlements
                .CountAsync(f => f.PaymentStatus != "Paid");

            return View(dashboardKpis);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
