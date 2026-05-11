using FinanceManager2._0.Controllers;
using FinanceManager2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinanceManager.Tests;

public class RecurringPaymentsTests
{
    [Fact]
    public async Task Create_ValidRecurringPayment_SavesForCurrentUserAndRedirects()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedCategoriesAsync(context);
        var controller = new RecurringController(context);
        TestHelpers.AttachControllerContext(controller, "user1");

        var result = await controller.Create(new RecurringPayment
        {
            Name = "Internet",
            Amount = 300,
            CategoryId = 1,
            StartDate = new DateTime(2024, 1, 1),
            Frequency = "Monthly",
            IsActive = true
        });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        var saved = await context.RecurringPayments.SingleAsync();
        Assert.Equal("user1", saved.UserId);
        Assert.Equal(DateTimeKind.Utc, saved.StartDate.Kind);
        Assert.Equal("Monthly", saved.Frequency);
    }

    [Fact]
    public async Task Create_InvalidCategory_ReturnsViewAndDoesNotSave()
    {
        await using var context = TestHelpers.CreateContext();
        var controller = new RecurringController(context);
        TestHelpers.AttachControllerContext(controller, "user1");

        var result = await controller.Create(new RecurringPayment
        {
            Name = "Bad",
            Amount = 10,
            CategoryId = 999,
            StartDate = DateTime.UtcNow,
            Frequency = "Weekly",
            IsActive = true
        });

        Assert.IsType<ViewResult>(result);
        Assert.Empty(context.RecurringPayments);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Index_ReturnsOnlyCurrentUserPayments()
    {
        await using var context = TestHelpers.CreateContext();
        await TestHelpers.SeedCategoriesAsync(context);
        context.RecurringPayments.AddRange(
            new RecurringPayment { Id = 1, UserId = "user1", CategoryId = 1, Name = "Own", Amount = 100, StartDate = DateTime.UtcNow, Frequency = "Monthly" },
            new RecurringPayment { Id = 2, UserId = "user2", CategoryId = 1, Name = "Other", Amount = 200, StartDate = DateTime.UtcNow, Frequency = "Monthly" }
        );
        await context.SaveChangesAsync();
        var controller = new RecurringController(context);
        TestHelpers.AttachControllerContext(controller, "user1");

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var payments = Assert.IsAssignableFrom<IEnumerable<RecurringPayment>>(view.Model);
        var item = Assert.Single(payments);
        Assert.Equal("Own", item.Name);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesOnlyCurrentUsersPayment()
    {
        await using var context = TestHelpers.CreateContext();
        context.RecurringPayments.AddRange(
            new RecurringPayment { Id = 1, UserId = "user1", CategoryId = 1, Name = "Own", Amount = 100, StartDate = DateTime.UtcNow, Frequency = "Monthly" },
            new RecurringPayment { Id = 2, UserId = "user2", CategoryId = 1, Name = "Other", Amount = 200, StartDate = DateTime.UtcNow, Frequency = "Monthly" }
        );
        await context.SaveChangesAsync();
        var controller = new RecurringController(context);
        TestHelpers.AttachControllerContext(controller, "user1");

        await controller.DeleteConfirmed(1);
        await controller.DeleteConfirmed(2);

        Assert.False(await context.RecurringPayments.AnyAsync(r => r.Id == 1));
        Assert.True(await context.RecurringPayments.AnyAsync(r => r.Id == 2));
    }

    [Fact]
    public void RecurringPayment_DefaultsAreSafe()
    {
        var payment = new RecurringPayment();

        Assert.Equal("Monthly", payment.Frequency);
        Assert.True(payment.IsActive);
        Assert.Equal(DateTimeKind.Utc, payment.StartDate.Kind);
    }
}
