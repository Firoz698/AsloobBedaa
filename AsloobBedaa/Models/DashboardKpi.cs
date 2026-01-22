namespace AsloobBedaa.Models
{
    public class DashboardKpi
    {
        public int Id { get; set; }

        public string KpiName { get; set; }   // Revenue, Net Profit etc.

        public decimal CurrentMonth { get; set; }
        public decimal PreviousMonth { get; set; }
        public decimal YearToDate { get; set; }

        public string Status { get; set; }    // Increase / Decrease / Stable
        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
