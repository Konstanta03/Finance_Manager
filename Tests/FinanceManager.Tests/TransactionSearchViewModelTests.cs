using FinanceManager2._0.ViewModels;
using Xunit;

namespace FinanceManager.Tests;

public class TransactionSearchViewModelTests
{
    [Fact]
    public void EmptySearchKeyword_IsAllowed()
    {
        var filter = new TransactionFilterViewModel();

        Assert.Null(filter.SearchKeyword);
    }
}
