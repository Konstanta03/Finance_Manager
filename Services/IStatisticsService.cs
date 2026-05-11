using FinanceManager2._0.Models;

namespace FinanceManager2._0.Services
{
    public interface IStatisticsService
    {
        Task<CompareExpensesViewModel> CompareAsync(string userId, DateTime currentStart, DateTime currentEnd, DateTime previousStart, DateTime previousEnd);
    }
}
