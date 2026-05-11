using FinanceManager2._0.Models;
using FinanceManager2._0.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinanceManager.Tests;

public class TransactionServiceTests
{
    [Fact]
    public async Task BuildQuery_ForRegularUser_ReturnsOnlyOwnTransactions_OrderedByNewestFirst()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedCategoriesAsync(context);
        context.Transactions.AddRange(
            new Transaction { Id = 1, UserId = "user1", CategoryId = 1, Amount = 20, Note = "old", Date = new DateTime(2024, 1, 1) },
            new Transaction { Id = 2, UserId = "user2", CategoryId = 1, Amount = 10, Note = "other", Date = new DateTime(2024, 3, 1) },
            new Transaction { Id = 3, UserId = "user1", CategoryId = 3, Amount = 40, Note = "new", Date = new DateTime(2024, 2, 1) }
        );
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var result = await service.BuildQuery("user1", false).ToListAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal("user1", t.UserId));
        Assert.Equal(new[] { 3, 1 }, result.Select(t => t.Id));
    }

    [Fact]
    public async Task BuildQuery_ForAdmin_ReturnsAllUsersTransactions()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedCategoriesAsync(context);
        context.Transactions.AddRange(
            new Transaction { UserId = "user1", CategoryId = 1, Amount = 20, Date = DateTime.UtcNow },
            new Transaction { UserId = "user2", CategoryId = 1, Amount = 10, Date = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var result = await service.BuildQuery("admin", true).ToListAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CreateAsync_AssignsCurrentUserAndStoresUtcDate()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedCategoriesAsync(context);
        var localDate = new DateTime(2024, 4, 10, 12, 0, 0, DateTimeKind.Local);

        var service = new TransactionService(context);
        await service.CreateAsync(new Transaction { CategoryId = 1, Amount = 99, Date = localDate, Note = "created" }, "user1");

        var saved = await context.Transactions.SingleAsync();
        Assert.Equal("user1", saved.UserId);
        Assert.Equal(DateTimeKind.Utc, saved.Date.Kind);
        Assert.Equal(99, saved.Amount);
    }

    [Fact]
    public async Task GetForEditAsync_ReturnsTransactionForOwnerOrAdminOnly()
    {
        await using var context = TestHelpers.CreateContext();
        context.Transactions.Add(new Transaction { Id = 5, UserId = "owner", CategoryId = 1, Amount = 20, Date = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var service = new TransactionService(context);

        Assert.NotNull(await service.GetForEditAsync(5, "owner", false));
        Assert.NotNull(await service.GetForEditAsync(5, "admin", true));
        Assert.Null(await service.GetForEditAsync(5, "stranger", false));
        Assert.Null(await service.GetForEditAsync(999, "owner", true));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesOwnTransactionAndBlocksOtherUsers()
    {
        await using var context = TestHelpers.CreateContext();
        context.Transactions.Add(new Transaction { Id = 1, UserId = "user1", CategoryId = 1, Amount = 20, Date = DateTime.UtcNow, Note = "old" });
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var blocked = await service.UpdateAsync(1, new Transaction { Id = 1, CategoryId = 3, Amount = 999, Date = DateTime.UtcNow, Note = "bad" }, "user2", false);
        var updated = await service.UpdateAsync(1, new Transaction { Id = 1, CategoryId = 3, Amount = 55, Date = new DateTime(2024, 5, 1), Note = "new" }, "user1", false);

        var saved = await context.Transactions.SingleAsync(t => t.Id == 1);
        Assert.False(blocked);
        Assert.True(updated);
        Assert.Equal(55, saved.Amount);
        Assert.Equal(3, saved.CategoryId);
        Assert.Equal("new", saved.Note);
    }

    [Fact]
    public async Task DeleteAsync_DeletesOwnTransactionAndBlocksOtherUsers()
    {
        await using var context = TestHelpers.CreateContext();
        context.Transactions.AddRange(
            new Transaction { Id = 1, UserId = "user1", CategoryId = 1, Amount = 20, Date = DateTime.UtcNow },
            new Transaction { Id = 2, UserId = "user2", CategoryId = 1, Amount = 30, Date = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var blocked = await service.DeleteAsync(2, "user1", false);
        var deleted = await service.DeleteAsync(1, "user1", false);

        Assert.False(blocked);
        Assert.True(deleted);
        Assert.False(await context.Transactions.AnyAsync(t => t.Id == 1));
        Assert.True(await context.Transactions.AnyAsync(t => t.Id == 2));
    }
}
