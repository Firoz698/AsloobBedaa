using AsloobBedaa.Common;

namespace AsloobBedaa.Models
{
    public class Project : Base
    {
        public string ProjectName { get; set; }
        public string? Location { get; set; }

        // Navigation
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
    }

}
