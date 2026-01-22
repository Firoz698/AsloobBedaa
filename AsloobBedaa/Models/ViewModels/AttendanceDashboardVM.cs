namespace AsloobBedaa.Models.ViewModels
{
    public class AttendanceDashboardVM
    {
        public string ProjectName { get; set; }

        // Day Shift
        public int DayBasic { get; set; }
        public int DayHourly { get; set; }
        public int DayTotalPresent { get; set; }
        public int DayWorkforce { get; set; }
        public int DayAbsent { get; set; }

        // Night Shift
        public int NightBasic { get; set; }
        public int NightHourly { get; set; }
        public int NightTotalPresent { get; set; }
        public int NightWorkforce { get; set; }
        public int NightAbsent { get; set; }
    }

}
