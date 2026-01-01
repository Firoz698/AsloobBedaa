using System;

namespace AsloobBedaa.Models
{
    public class Overtime
    {
        public int OvertimeID { get; set; }               // Primary Key
        public int AttendanceID { get; set; }             // Foreign Key to Attendance
        public Attendance ?Attendance { get; set; }       // Navigation property

        public decimal OvertimeHours { get; set; }       // Extra hours worked
        public decimal OvertimeAmount { get; set; }      // Payment for overtime

        public DateTime CreatedAt { get; set; } = DateTime.Now;   // Record creation time
        public DateTime? UpdatedAt { get; set; }                   // Last update time
        public bool IsDeleted { get; set; } = false;              // Soft delete flag

        // Method to calculate overtime payment
        public void CalculateOvertimeAmount(decimal overtimeRate)
        {
            OvertimeAmount = OvertimeHours * overtimeRate;
        }
    }
}
