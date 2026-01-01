namespace AsloobBedaa.Models
{
    public class Project
    {
        public int ProjectID { get; set; }
        public string? ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }

}
