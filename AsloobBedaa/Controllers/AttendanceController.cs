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

        // Index view
        public IActionResult Index()
        {
            ViewBag.Employees = new SelectList(
                _context.Employees.Where(e => !e.IsDeleted),
                "EmployeeID",
                "Name"
            );

            ViewBag.Projects = new SelectList(
                _context.Projects,
                "ProjectID",
                "ProjectName"
            );

            ViewBag.AttendanceList = _context.Attendances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => !a.IsDeleted)
                .ToList();

            return View();
        }


        // Save attendance
        [HttpPost]
        public IActionResult SaveAttendance(Attendance model)
        {
            if (model == null || model.StartTime == DateTime.MinValue || model.EndTime == DateTime.MinValue)
            {
                return BadRequest("Invalid attendance data.");
            }


            _context.Attendances.Add(model);
            _context.SaveChanges();

            return Ok(new { message = "Attendance saved successfully." });
        }


    }
}
