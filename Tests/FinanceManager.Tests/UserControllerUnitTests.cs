using FinanceManager2._0.Controllers;
using FinanceManager2._0.Models;
using FinanceManager2._0.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FinanceManager.Tests;

public class UserControllerUnitTests
{
    [Fact]
    public async Task Login_WithValidCredentials_RedirectsToTransactions()
    {
        var userManager = TestHelpers.MockUserManager();
        var signInManager = TestHelpers.MockSignInManager(userManager.Object);
        var roleManager = TestHelpers.MockRoleManager();

        signInManager
            .Setup(s => s.PasswordSignInAsync("admin@example.com", "Admin123", false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var controller = new UserController(userManager.Object, signInManager.Object, roleManager.Object);
        TestHelpers.AttachControllerContext(controller);

        var result = await controller.Login(new LoginViewModel { Email = "admin@example.com", Password = "Admin123" }) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result!.ActionName);
        Assert.Equal("Transactions", result.ControllerName);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsViewAndAddsModelError()
    {
        var userManager = TestHelpers.MockUserManager();
        var signInManager = TestHelpers.MockSignInManager(userManager.Object);
        var roleManager = TestHelpers.MockRoleManager();

        signInManager
            .Setup(s => s.PasswordSignInAsync("bad@example.com", "bad", false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var controller = new UserController(userManager.Object, signInManager.Object, roleManager.Object);
        TestHelpers.AttachControllerContext(controller);

        var result = await controller.Login(new LoginViewModel { Email = "bad@example.com", Password = "bad" });

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Register_WhenIdentitySucceeds_CreatesUserAddsRoleSignsInAndRedirects()
    {
        var userManager = TestHelpers.MockUserManager();
        var signInManager = TestHelpers.MockSignInManager(userManager.Object);
        var roleManager = TestHelpers.MockRoleManager();

        roleManager.Setup(r => r.RoleExistsAsync("User")).ReturnsAsync(true);
        userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), "Test123"))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);
        signInManager.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), false, null))
            .Returns(Task.CompletedTask);

        var controller = new UserController(userManager.Object, signInManager.Object, roleManager.Object);
        TestHelpers.AttachControllerContext(controller);

        var result = await controller.Register(new RegisterViewModel
        {
            Email = "new@example.com",
            Password = "Test123",
            ConfirmPassword = "Test123"
        }) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result!.ActionName);
        Assert.Equal("Transactions", result.ControllerName);
        userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
        signInManager.Verify(s => s.SignInAsync(It.IsAny<ApplicationUser>(), false, null), Times.Once);
    }

    [Fact]
    public async Task Register_WhenRoleDoesNotExist_CreatesUserRole()
    {
        var userManager = TestHelpers.MockUserManager();
        var signInManager = TestHelpers.MockSignInManager(userManager.Object);
        var roleManager = TestHelpers.MockRoleManager();

        roleManager.Setup(r => r.RoleExistsAsync("User")).ReturnsAsync(false);
        roleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
        userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), "Test123"))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);
        signInManager.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), false, null))
            .Returns(Task.CompletedTask);

        var controller = new UserController(userManager.Object, signInManager.Object, roleManager.Object);
        TestHelpers.AttachControllerContext(controller);

        await controller.Register(new RegisterViewModel { Email = "new@example.com", Password = "Test123", ConfirmPassword = "Test123" });

        roleManager.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == "User")), Times.Once);
    }
}
