namespace FinanceManager2._0.ViewModels
{
    public class TransactionFilterViewModel
    {
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
    }
}
