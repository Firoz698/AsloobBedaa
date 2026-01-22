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

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ActivityLogger actilogger)
        {
            _logger = logger;
            _context = context;
            _actilogger = actilogger;
        }

        public async Task<IActionResult> Index()
        {
            var name = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = name;

            // Dashboard KPI list
            var dashboardKpis = await _context.DashboardKpis.ToListAsync();

            // Active Projects
            ViewBag.ActiveProjects = await _context.Subcontractors
                                             .Where(s => s.Status == "Active")
                                             .CountAsync();

            // Monthly Payroll - client-side evaluation to avoid EF Core ToString() issue
            var currentMonthString = DateTime.Now.ToString("MMM-yyyy");
            ViewBag.MonthlyPayroll = _context.PayrollMonthlies
                                      .AsEnumerable()  // brings data to memory
                                      .Where(p => p.Month == currentMonthString)
                                      .Sum(p => p.NetPayrollCost);

            // Accounts Receivable & Payable
            ViewBag.AccountsReceivable = await _context.AccountsTransactions
                                                .Where(a => a.Type == "AR")
                                                .SumAsync(a => a.BalanceAmount);

            ViewBag.AccountsPayable = await _context.AccountsTransactions
                                                .Where(a => a.Type == "AP")
                                                .SumAsync(a => a.BalanceAmount);

            // Pending Final Settlements
            ViewBag.PendingSettlements = await _context.FinalSettlements
                                               .Where(f => f.PaymentStatus != "Paid")
                                               .CountAsync();

            return View(dashboardKpis);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
