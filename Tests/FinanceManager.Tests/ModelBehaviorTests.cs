using FinanceManager2._0.Models;
using Xunit;

namespace FinanceManager.Tests;

public class ModelBehaviorTests
{
    [Fact]
    public void Transaction_DateSetter_NormalizesKindToUtc()
    {
        var transaction = new Transaction
        {
            Date = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Unspecified)
        };

        Assert.Equal(DateTimeKind.Utc, transaction.Date.Kind);
    }

    [Fact]
    public void Settings_DefaultValues_AreSafeForNewUser()
    {
        var settings = new Settings();

        Assert.Equal("USD", settings.Currency);
        Assert.Equal("uk", settings.Language);
        Assert.Equal("light", settings.Theme);
        Assert.True(settings.SaveSettings);
    }

    [Fact]
    public void Category_TransactionsCollection_IsInitialized()
    {
        var category = new Category();

        Assert.NotNull(category.Transactions);
        Assert.Empty(category.Transactions);
    }
}
