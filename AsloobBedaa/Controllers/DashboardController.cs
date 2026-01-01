using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsloobBedaa.Models;
using AsloobBedaa.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;
using AsloobBedaa.DataContext;

namespace AsloobBedaa.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            // Total Employees Today
            var totalEmployees = await _context.Attendances
                                    .Where(a => a.Date == today && !a.IsDeleted)
                                    .Select(a => a.EmployeeID)
                                    .Distinct()
                                    .CountAsync();

            // Total Projects Today
            var totalProjects = await _context.Attendances
                                    .Where(a => a.Date == today && !a.IsDeleted)
                                    .Select(a => a.ProjectID)
                                    .Distinct()
                                    .CountAsync();

            // Total Daily Expense (Worked + Overtime)
            var totalExpense = await _context.Attendances
                                    .Where(a => a.Date == today && !a.IsDeleted)
                                    .SumAsync(a => a.DailySalary + a.OvertimeHours * a.Employee.OvertimeRate);

            // Total Overtime Expense
            var totalOvertimeExpense = await _context.Attendances
                                        .Where(a => a.Date == today && !a.IsDeleted)
                                        .SumAsync(a => a.OvertimeHours * a.Employee.OvertimeRate);

            // Project-wise Summary
            var projectSummaries = await _context.Attendances
                .Where(a => a.Date == today && !a.IsDeleted)
                .GroupBy(a => new { a.ProjectID, a.Project.ProjectName })
                .Select(g => new ProjectSummary
                {
                    ProjectID = g.Key.ProjectID,
                    ProjectName = g.Key.ProjectName,
                    EmployeesWorking = g.Select(x => x.EmployeeID).Distinct().Count(),
                    ProjectDailyCost = g.Sum(x => x.DailySalary),
                    ProjectOvertimeCost = g.Sum(x => x.OvertimeHours * x.Employee.OvertimeRate)
                })
                .ToListAsync();

            // Prepare Dashboard ViewModel
            var dashboardVM = new DashboardVM
            {
                TotalEmployeesToday = totalEmployees,
                TotalProjectsToday = totalProjects,
                TotalExpenseToday = totalExpense,
                TotalOvertimeExpense = totalOvertimeExpense,
                ProjectSummaries = projectSummaries
            };

            return View(dashboardVM);
        }
    }
}
