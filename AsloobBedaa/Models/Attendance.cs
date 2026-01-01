namespace AsloobBedaa.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        public int ProjectID { get; set; }
        public Project Project { get; set; }

        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal WorkedHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal DailySalary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Method to calculate hours and salary
        public void CalculateSalary(decimal hourlyRate, decimal overtimeRate, decimal standardHours = 8)
        {
            WorkedHours = (decimal)(EndTime - StartTime).TotalHours;
            OvertimeHours = WorkedHours > standardHours ? WorkedHours - standardHours : 0;
            if (OvertimeHours > 0) WorkedHours = standardHours;

            DailySalary = (WorkedHours * hourlyRate) + (OvertimeHours * overtimeRate);
        }
    }


}
