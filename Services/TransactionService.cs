using FinanceManager2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager2._0.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly FinanceDbContext _context;

        public TransactionService(FinanceDbContext context)
        {
            _context = context;
        }

        public IQueryable<Transaction> BuildQuery(string userId, bool isAdmin)
        {
            IQueryable<Transaction> query = _context.Transactions.Include(t => t.Category);

            if (!isAdmin)
                query = query.Where(t => t.UserId == userId);

            return query.OrderByDescending(t => t.Date);
        }

        public async Task<Transaction?> GetForEditAsync(int id, string userId, bool isAdmin)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return null;
            if (!isAdmin && transaction.UserId != userId) return null;
            return transaction;
        }

        public async Task CreateAsync(Transaction transaction, string userId)
        {
            transaction.UserId = userId;
            transaction.Date = DateTime.SpecifyKind(transaction.Date == default ? DateTime.UtcNow : transaction.Date, DateTimeKind.Utc);
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, Transaction form, string userId, bool isAdmin)
        {
            var existing = await _context.Transactions.FindAsync(id);
            if (existing == null) return false;
            if (!isAdmin && existing.UserId != userId) return false;

            existing.CategoryId = form.CategoryId;
            existing.Amount = form.Amount;
            existing.Date = DateTime.SpecifyKind(form.Date == default ? DateTime.UtcNow : form.Date, DateTimeKind.Utc);
            existing.Note = form.Note;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;
            if (!isAdmin && transaction.UserId != userId) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
