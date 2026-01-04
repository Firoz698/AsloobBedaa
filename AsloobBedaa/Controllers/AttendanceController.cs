using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AsloobBedaa.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Index view with filter, search & table
        public IActionResult Index()
        {
            // Employees for dropdown
            ViewBag.Employees = new SelectList(
                _context.Employees.Where(e => !e.IsDeleted).ToList(),
                "EmployeeID",
                "Name"
            );

            // Projects for dropdown
            ViewBag.Projects = new SelectList(
                _context.Projects.Where(p => !p.IsDeleted).ToList(),
                "ProjectID",
                "ProjectName"
            );

            // Attendance List with Employee & Project included
            ViewBag.AttendanceList = _context.Attendances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.Date)
                .ToList();

            return View();
        }

        // POST: Save Attendance via AJAX
        [HttpPost]
        public IActionResult SaveAttendance([FromBody] Attendance model)
        {
            if (model == null || model.StartTime == DateTime.MinValue || model.EndTime == DateTime.MinValue)
            {
                return BadRequest(new { message = "Invalid attendance data." });
            }

            // Get employee to calculate salary
            var employee = _context.Employees.Find(model.EmployeeID);
            if (employee == null)
            {
                return BadRequest(new { message = "Employee not found." });
            }

            // Calculate WorkedHours, OvertimeHours & DailySalary
            model.CalculateSalary(employee.HourlyRate, employee.OvertimeRate);

            // Set CreatedAt
            model.CreatedAt = DateTime.Now;

            // Add to DB
            _context.Attendances.Add(model);
            _context.SaveChanges();

            return Ok(new { message = "Attendance saved successfully." });
        }

        // Optional: Delete Attendance (Soft delete)
        [HttpPost]
        public IActionResult DeleteAttendance(int id)
        {
            var attendance = _context.Attendances.Find(id);
            if (attendance == null)
            {
                return NotFound(new { message = "Attendance not found." });
            }

            attendance.IsDeleted = true;
            attendance.UpdatedAt = DateTime.Now;

            _context.Update(attendance);
            _context.SaveChanges();

            return Ok(new { message = "Attendance deleted successfully." });
        }
    }
}
