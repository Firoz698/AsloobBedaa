namespace AsloobBedaa.Models
{
    public class PayrollCSVModel
    {
        public List<PayrollMonthly> MatchedPayrolls { get; set; } = new List<PayrollMonthly>();
        public List<PayrollMonthly> NonMatchedPayrolls { get; set; } = new List<PayrollMonthly>();
    }
}
