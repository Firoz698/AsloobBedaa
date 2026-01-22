namespace AsloobBedaa.Models
{
    public class Subcontractor
    {
        public int SubcontractorId { get; set; }

        public string SubcontractorName { get; set; }
        public string ProjectName { get; set; }

        public decimal ContractValue { get; set; }
        public decimal MonthlyBilling { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal OutstandingBalance { get; set; }

        public decimal RetentionAmount { get; set; }

        public string Status { get; set; }   // Active, Completed, On Hold
        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
