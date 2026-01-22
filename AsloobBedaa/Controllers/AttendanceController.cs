using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AsloobBedaa.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    ViewBag.Employees = new SelectList(
        //        _context.Employees.Where(e => !e.IsDeleted).ToList(),
        //        "EmployeeID",
        //        "Name"
        //    );

        //    ViewBag.Projects = new SelectList(
        //        _context.Projects.Where(p => !p.IsDeleted).ToList(),
        //        "ProjectID",
        //        "ProjectName"
        //    );

        //    ViewBag.AttendanceList = _context.Attendances
        //        .Include(a => a.Employee)
        //        .Include(a => a.Project)
        //        .Where(a => !a.IsDeleted)
        //        .OrderByDescending(a => a.Date)
        //        .ToList();

        //    return View();
        //}

        //[HttpPost]
        //public IActionResult SaveAttendance([FromBody] Attendance model)
        //{
        //    if (model == null)
        //        return BadRequest(new { message = "Invalid data" });

        //    var employee = _context.Employees.Find(model.EmployeeID);
        //    if (employee == null)
        //        return BadRequest(new { message = "Employee not found" });

        //    model.CalculateSalary(employee.HourlyRate, employee.OvertimeRate);
        //    model.CreatedAt = DateTime.Now;

        //    _context.Attendances.Add(model);
        //    _context.SaveChanges();

        //    return Ok(new { message = "Attendance saved successfully" });
        //}

        [HttpPost]
        public IActionResult DeleteAttendance(int id)
        {
            var attendance = _context.Attendances.Find(id);
            if (attendance == null)
                return NotFound();

            attendance.IsDeleted = true;
            attendance.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok();
        }
    }
}
