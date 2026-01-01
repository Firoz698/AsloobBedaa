using System;
using System.Collections.Generic;

namespace AsloobBedaa.ViewModels
{
    public class DashboardVM
    {
        // Summary Info
        public int TotalEmployeesToday { get; set; }
        public int TotalProjectsToday { get; set; }
        public decimal TotalExpenseToday { get; set; }
        public decimal TotalOvertimeExpense { get; set; }

        // Optional: Average worked hours today
        public decimal AverageWorkedHours { get; set; }

        // Project-wise Summary
        public List<ProjectSummary> ProjectSummaries { get; set; } = new List<ProjectSummary>();
    }

    // Class to show project-wise details on dashboard
    public class ProjectSummary
    {
        public int ProjectID { get; set; }
        public string ?ProjectName { get; set; }
        public int EmployeesWorking { get; set; }
        public decimal ProjectDailyCost { get; set; }
        public decimal ProjectOvertimeCost { get; set; }
    }
}
