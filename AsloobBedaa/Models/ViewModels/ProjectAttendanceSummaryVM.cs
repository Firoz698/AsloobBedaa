namespace AsloobBedaa.Models.ViewModels
{
    public class ProjectAttendanceSummaryVM
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }

        // Day Shift (Today)
        public int DayBasic { get; set; }
        public int DayHourly { get; set; }
        public int DayTotalPresent => DayBasic + DayHourly;
        public int DayWorkforce { get; set; }
        public int DayAbsent => DayWorkforce - DayTotalPresent;

        // Night Shift (Yesterday)
        public int NightBasic { get; set; }
        public int NightHourly { get; set; }
        public int NightTotalPresent => NightBasic + NightHourly;
        public int NightWorkforce { get; set; }
        public int NightAbsent => NightWorkforce - NightTotalPresent;
    }
}
