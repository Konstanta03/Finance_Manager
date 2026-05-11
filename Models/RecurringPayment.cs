using System;

namespace FinanceManager2._0.Models
{
    public class RecurringPayment
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Amount { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        private DateTime _startDate = DateTime.UtcNow;
        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public string Frequency { get; set; } = "Monthly";
        public bool IsActive { get; set; } = true;
    }
}