using AsloobBedaa.Common;
using System;
using System.Collections.Generic;

namespace AsloobBedaa.Models
{
    public class Shift : Base
    {
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public ICollection<Attendance>? Attendances { get; set; }
    }
}
