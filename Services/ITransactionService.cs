using FinanceManager2._0.Models;

namespace FinanceManager2._0.Services
{
    public interface ITransactionService
    {
        IQueryable<Transaction> BuildQuery(string userId, bool isAdmin);
        Task<Transaction?> GetForEditAsync(int id, string userId, bool isAdmin);
        Task CreateAsync(Transaction transaction, string userId);
        Task<bool> UpdateAsync(int id, Transaction form, string userId, bool isAdmin);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }
}
