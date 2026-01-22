namespace AsloobBedaa.Models
{
    public class FinalSettlement
    {
        public int SettlementId { get; set; }

        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }

        public string ProjectName { get; set; }
        public DateTime LastWorkingDay { get; set; }

        public decimal PendingSalary { get; set; }
        public decimal LeaveSalary { get; set; }
        public decimal Gratuity { get; set; }

        public decimal Deductions { get; set; }

        public decimal NetSettlementAmount { get; set; }

        public string PaymentStatus { get; set; }  // Paid / Pending
        public string Remarks { get; set; }
    }

}
