namespace AsloobBedaa.Models
{
    public class PayrollCSVModel
    {
        public IFormFile CsvFile { get; set; }
        public List<PayrollMonthly> MatchedPayrolls { get; set; } = new();
        public List<PayrollMonthly> NonMatchedPayrolls { get; set; } = new();
    }

}
