namespace FinanceManager2._0.ViewModels
{
    public class TransactionFilterViewModel
    {
        public string SortBy { get; set; } = "date";
        public string SortOrder { get; set; } = "desc";
        public string? SearchKeyword { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
    }
}
