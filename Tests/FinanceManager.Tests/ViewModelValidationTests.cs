using System.ComponentModel.DataAnnotations;
using FinanceManager2._0.ViewModels;
using Xunit;

namespace FinanceManager.Tests;

public class ViewModelValidationTests
{
    private static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void RegisterViewModel_Requires_ValidEmailPasswordAndConfirmation()
    {
        var model = new RegisterViewModel
        {
            Email = "wrong-email",
            Password = "123",
            ConfirmPassword = "456"
        };

        var errors = Validate(model);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(RegisterViewModel.Email)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(RegisterViewModel.Password)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(RegisterViewModel.ConfirmPassword)));
    }

    [Fact]
    public void RegisterViewModel_AcceptsValidData()
    {
        var model = new RegisterViewModel
        {
            Email = "user@example.com",
            Password = "Test123",
            ConfirmPassword = "Test123"
        };

        Assert.Empty(Validate(model));
    }

    [Fact]
    public void LoginViewModel_RequiresEmailAndPassword()
    {
        var model = new LoginViewModel();

        var errors = Validate(model);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(LoginViewModel.Email)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(LoginViewModel.Password)));
    }

    [Fact]
    public void LoginViewModel_Accepts_ValidData()
    {
        var model = new LoginViewModel
        {
            Email = "user@example.com",
            Password = "Test123"
        };

        Assert.Empty(Validate(model));
    }
}
