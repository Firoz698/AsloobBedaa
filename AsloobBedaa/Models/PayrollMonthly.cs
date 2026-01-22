namespace AsloobBedaa.Models
{
    public class PayrollMonthly
    {
        public int PayrollId { get; set; }

        public string Month { get; set; }     // e.g. Jan-2026
        public string ProjectName { get; set; }

        public int NumberOfEmployees { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal Overtime { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }

        public decimal NetPayrollCost { get; set; }

        public string WpsStatus { get; set; }   // Submitted / Pending
        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
