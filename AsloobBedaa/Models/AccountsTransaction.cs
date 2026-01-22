namespace AsloobBedaa.Models
{
    public class AccountsTransaction
    {
        public int Id { get; set; }

        public string Type { get; set; }   // AR or AP

        public string ClientOrVendorName { get; set; }

        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }

        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }

        public decimal BalanceAmount { get; set; }

        public DateTime DueDate { get; set; }
        public int AgingDays { get; set; }

        public string Status { get; set; }   // Paid, Partial, Overdue
        public string Remarks { get; set; }
    }
}
