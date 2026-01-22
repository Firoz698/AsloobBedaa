using AsloobBedaa.Common;

namespace AsloobBedaa.Models
{
    public class Attendance : Base
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int ShiftId { get; set; }
        public Shift Shift { get; set; }

        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
    }


}
