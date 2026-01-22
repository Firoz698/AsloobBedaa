using AsloobBedaa.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsloobBedaa.Models
{
    public class Employee : Base
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmployeeType { get; set; } // Basic / Hourly

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        // Navigation
        public ICollection<Attendance> Attendances { get; set; }
    }
}
