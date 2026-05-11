using FinanceManager2._0.Controllers;
using FinanceManager2._0.Models;
using FinanceManager2._0.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinanceManager.Tests;

public class ControllerUnitTests
{
    [Fact]
    public async Task Categories_CreateValidCategory_SavesAndRedirects()
    {
        await using var context = TestHelpers.CreateContext();
        var controller = new CategoriesController(context);
        TestHelpers.AttachControllerContext(controller);

        var result = await controller.Create(new Category { Name = "Food", IsExpense = true });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.True(await context.Categories.AnyAsync(c => c.Name == "Food"));
    }

    [Fact]
    public async Task Categories_EditWithWrongId_ReturnsNotFound()
    {
        await using var context = TestHelpers.CreateContext();
        var controller = new CategoriesController(context);
        TestHelpers.AttachControllerContext(controller);

        var result = await controller.Edit(5, new Category { Id = 7, Name = "Wrong", IsExpense = true });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Categories_DeleteConfirmed_RemovesCategory()
    {
        await using var context = TestHelpers.CreateContext();
        context.Categories.Add(new Category { Id = 1, Name = "Food", IsExpense = true });
        await context.SaveChangesAsync();
        var controller = new CategoriesController(context);
        TestHelpers.AttachControllerContext(controller);

        var result = await controller.DeleteConfirmed(1);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.False(await context.Categories.AnyAsync(c => c.Id == 1));
    }

    [Fact]
    public async Task Settings_PostCreatesSettingsForCurrentUser()
    {
        await using var context = TestHelpers.CreateContext();
        var user = new ApplicationUser { Id = "user1", Email = "user@example.com", UserName = "user@example.com" };
        var userManager = TestHelpers.MockUserManager(user);
        var controller = new SettingsController(context, userManager.Object);
        TestHelpers.AttachControllerContext(controller, user.Id);

        var result = await controller.Index(new Settings
        {
            Currency = "UAH",
            Language = "uk",
            Theme = "dark",
            SaveSettings = true,
            ExportData = "json"
        });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        var saved = await context.Settings.SingleAsync();
        Assert.Equal("user1", saved.UserId);
        Assert.Equal("UAH", saved.Currency);
        Assert.Equal("dark", saved.Theme);
    }

    [Fact]
    public async Task Transactions_CreateInvalidCategory_ReturnsViewAndDoesNotSave()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedCategoriesAsync(context);
        var user = new ApplicationUser { Id = "user1", Email = "user@example.com", UserName = "user@example.com" };
        var userManager = TestHelpers.MockUserManager(user);
        var service = new TransactionService(context);
        var controller = new TransactionsController(context, userManager.Object, service);
        TestHelpers.AttachControllerContext(controller, user.Id);

        var result = await controller.Create(new Transaction { CategoryId = 999, Amount = 10, Date = DateTime.UtcNow });

        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<Transaction>(view.Model);
        Assert.Empty(context.Transactions);
        Assert.False(controller.ModelState.IsValid);
    }
}
