using FinanceManager2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager2._0.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly FinanceDbContext _context;

        public StatisticsService(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<CompareExpensesViewModel> CompareAsync(string userId, DateTime currentStart, DateTime currentEnd, DateTime previousStart, DateTime previousEnd)
        {
            currentStart = DateTime.SpecifyKind(currentStart.Date, DateTimeKind.Utc);
            currentEnd = DateTime.SpecifyKind(currentEnd.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            previousStart = DateTime.SpecifyKind(previousStart.Date, DateTimeKind.Utc);
            previousEnd = DateTime.SpecifyKind(previousEnd.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var currentQuery = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= currentStart && t.Date <= currentEnd && t.Category != null && t.Category.IsExpense);

            var previousQuery = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= previousStart && t.Date <= previousEnd && t.Category != null && t.Category.IsExpense);

            var currentTotal = await currentQuery.SumAsync(t => t.Amount);
            var previousTotal = await previousQuery.SumAsync(t => t.Amount);

            var currentByCategory = await currentQuery
                .GroupBy(t => t.Category!.Name)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                .ToListAsync();

            var previousByCategory = await previousQuery
                .GroupBy(t => t.Category!.Name)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                .ToListAsync();

            var model = new CompareExpensesViewModel
            {
                CurrentPeriodTotal = currentTotal,
                PreviousPeriodTotal = previousTotal
            };

            foreach (var category in currentByCategory.Select(c => c.Category).Union(previousByCategory.Select(c => c.Category)).OrderBy(c => c))
            {
                model.ExpensesByCategory.Add(new ExpenseByCategory
                {
                    CategoryName = category,
                    CurrentAmount = currentByCategory.FirstOrDefault(c => c.Category == category)?.Amount ?? 0,
                    PreviousAmount = previousByCategory.FirstOrDefault(c => c.Category == category)?.Amount ?? 0
                });
            }

            return model;
        }
    }
}
