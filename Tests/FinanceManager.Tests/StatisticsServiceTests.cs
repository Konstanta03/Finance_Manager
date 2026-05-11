using FinanceManager2._0.Controllers;
using FinanceManager2._0.Models;
using FinanceManager2._0.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FinanceManager.Tests;

public class StatisticsServiceTests
{
    [Fact]
    public async Task CompareAsync_CountsOnlyCurrentUsersExpenseCategories()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedCategoriesAsync(context);
        context.Transactions.AddRange(
            new Transaction { UserId = "user1", CategoryId = 1, Amount = 100, Date = new DateTime(2024, 2, 10) },
            new Transaction { UserId = "user1", CategoryId = 3, Amount = 50, Date = new DateTime(2024, 2, 12) },
            new Transaction { UserId = "user1", CategoryId = 2, Amount = 999, Date = new DateTime(2024, 2, 14) },
            new Transaction { UserId = "user2", CategoryId = 1, Amount = 999, Date = new DateTime(2024, 2, 10) },
            new Transaction { UserId = "user1", CategoryId = 1, Amount = 40, Date = new DateTime(2024, 1, 10) }
        );
        await context.SaveChangesAsync();

        var service = new StatisticsService(context);
        var result = await service.CompareAsync("user1", new DateTime(2024, 2, 1), new DateTime(2024, 2, 29), new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.Equal(150, result.CurrentPeriodTotal);
        Assert.Equal(40, result.PreviousPeriodTotal);
        Assert.Equal(275, result.ChangePercent);
        Assert.DoesNotContain(result.ExpensesByCategory, c => c.CategoryName == "Salary");
    }

    [Fact]
    public async Task CompareAsync_IncludesCategoriesThatExistOnlyInPreviousPeriod()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedCategoriesAsync(context);
        context.Transactions.AddRange(
            new Transaction { UserId = "user1", CategoryId = 1, Amount = 100, Date = new DateTime(2024, 2, 10) },
            new Transaction { UserId = "user1", CategoryId = 3, Amount = 25, Date = new DateTime(2024, 1, 10) }
        );
        await context.SaveChangesAsync();

        var service = new StatisticsService(context);
        var result = await service.CompareAsync("user1", new DateTime(2024, 2, 1), new DateTime(2024, 2, 29), new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.Contains(result.ExpensesByCategory, c => c.CategoryName == "Food" && c.CurrentAmount == 100 && c.PreviousAmount == 0);
        Assert.Contains(result.ExpensesByCategory, c => c.CategoryName == "Transport" && c.CurrentAmount == 0 && c.PreviousAmount == 25);
    }

    [Fact]
    public void CompareExpensesViewModel_WhenPreviousTotalIsZero_ReturnsHundredPercent()
    {
        var model = new CompareExpensesViewModel
        {
            CurrentPeriodTotal = 250,
            PreviousPeriodTotal = 0
        };

        Assert.Equal(100, model.ChangePercent);
    }

    [Fact]
    public async Task StatisticsController_WithoutUser_RedirectsToLogin()
    {
        var service = new MockStatisticsService();
        var controller = new StatisticsController(service);
        TestHelpers.AttachControllerContext(controller, "");
        controller.ControllerContext.HttpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());

        var result = await controller.Compare(null, null, null, null);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("User", redirect.ControllerName);
    }

    private sealed class MockStatisticsService : IStatisticsService
    {
        public Task<CompareExpensesViewModel> CompareAsync(string userId, DateTime currentStart, DateTime currentEnd, DateTime previousStart, DateTime previousEnd)
        {
            return Task.FromResult(new CompareExpensesViewModel());
        }
    }
}
