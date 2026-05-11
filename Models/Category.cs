using System.Collections.Generic;

namespace FinanceManager2._0.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsExpense { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}