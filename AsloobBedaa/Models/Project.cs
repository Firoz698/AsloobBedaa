using System;
using System.Collections.Generic;

namespace AsloobBedaa.Models
{
    public class Project
    {
        public int ProjectID { get; set; }         // Primary Key
        public string ProjectName { get; set; }    // Project Name
        public DateTime? StartDate { get; set; }   // Optional Start Date
        public DateTime? EndDate { get; set; }     // Optional End Date
        public decimal? Budget { get; set; }       // Optional Budget

        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Creation timestamp
        public DateTime? UpdatedAt { get; set; }                // Update timestamp
        public bool IsDeleted { get; set; } = false;           // Soft delete flag

        // Navigation Properties
        public ICollection<Employee>? Employees { get; set; }
        public ICollection<Attendance>? Attendances { get; set; }
    }
}
