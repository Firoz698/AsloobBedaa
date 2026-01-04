using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsloobBedaa.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Employee name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [StringLength(100, ErrorMessage = "Position cannot exceed 100 characters")]
        public string? Position { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0, 999999, ErrorMessage = "Hourly rate must be a positive value")]
        public decimal HourlyRate { get; set; }

        [Required(ErrorMessage = "Overtime rate is required")]
        [Range(0, 999999, ErrorMessage = "Overtime rate must be a positive value")]
        public decimal OvertimeRate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<Attendance> Attendances { get; set; }
            = new List<Attendance>();
    }
}
