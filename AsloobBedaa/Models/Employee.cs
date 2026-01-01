namespace AsloobBedaa.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal OvertimeRate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }

}
