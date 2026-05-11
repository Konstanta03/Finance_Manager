using FinanceManager2._0.ViewModels;
using Xunit;

namespace FinanceManager.Tests;

public class TransactionFilterViewModelTests
{
    [Fact]
    public void EmptyFilter_HasNoRestrictions()
    {
        var filter = new TransactionFilterViewModel();

        Assert.Null(filter.MinAmount);
        Assert.Null(filter.MaxAmount);
        Assert.Null(filter.StartDate);
        Assert.Null(filter.EndDate);
        Assert.Null(filter.CategoryId);
    }
}
