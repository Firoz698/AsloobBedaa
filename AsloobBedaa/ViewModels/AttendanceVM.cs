using System;

namespace AsloobBedaa.ViewModels
{
    public class AttendanceVM
    {
        public int AttendanceID { get; set; }

        // Employee Info
        public int EmployeeID { get; set; }
        public string ?EmployeeName { get; set; }

        // Project Info
        public int ProjectID { get; set; }
        public string ?ProjectName { get; set; }

        // Attendance Info
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal WorkedHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal DailySalary { get; set; }

        // Optional: Overtime Payment (if separate)
        public decimal OvertimeAmount { get; set; }

        // Calculated Properties for UI convenience
        public string DisplayDate => Date.ToString("yyyy-MM-dd");
        public string DisplayStartTime => StartTime.ToString("HH:mm");
        public string DisplayEndTime => EndTime.ToString("HH:mm");
    }
}
