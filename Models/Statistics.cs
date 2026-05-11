using System;
using System.Collections.Generic;

namespace FinanceManager2._0.Models
{
    public class CompareExpensesViewModel
    {
        public decimal CurrentPeriodTotal { get; set; }
        public decimal PreviousPeriodTotal { get; set; }
        public decimal ChangePercent
        {
            get
            {
                if (PreviousPeriodTotal == 0) return 100;
                return Math.Round((CurrentPeriodTotal - PreviousPeriodTotal) / PreviousPeriodTotal * 100, 2);
            }
        }

        public List<ExpenseByCategory> ExpensesByCategory { get; set; } = new();
    }

    public class ExpenseByCategory
    {
        public string CategoryName { get; set; } = null!;
        public decimal CurrentAmount { get; set; }
        public decimal PreviousAmount { get; set; }
    }
}