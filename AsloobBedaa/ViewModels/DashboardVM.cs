using System;
using System.Collections.Generic;

namespace AsloobBedaa.ViewModels
{
    public class DashboardVM
    {
        public int TotalEmployeesToday { get; set; }
        public int TotalProjectsToday { get; set; }
        public decimal TotalExpenseToday { get; set; }
        public decimal TotalOvertimeExpense { get; set; }
        public decimal AverageWorkedHours { get; set; }

        public List<ProjectSummary> ProjectSummaries { get; set; } = new();
    }

    public class ProjectSummary
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int EmployeesWorking { get; set; }
        public decimal ProjectDailyCost { get; set; }
        public decimal ProjectOvertimeCost { get; set; }

        public decimal ProjectTotalCost => ProjectDailyCost + ProjectOvertimeCost;
    }

}
