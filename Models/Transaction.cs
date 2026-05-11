using System;

namespace FinanceManager2._0.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public decimal Amount { get; set; }

        private DateTime _date = DateTime.UtcNow;
        public DateTime Date
        {
            get => _date;
            set => _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public string? Note { get; set; }
    }
}