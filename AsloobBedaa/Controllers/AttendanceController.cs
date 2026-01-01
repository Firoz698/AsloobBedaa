using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
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

            // Calculate total hours
            TimeSpan duration = model.EndTime - model.StartTime;
            model.TotalHours = (decimal)duration.TotalHours;

            _context.Attendances.Add(model);
            _context.SaveChanges();

            return Ok(new { message = "Attendance saved successfully." });
        }


    }
}
