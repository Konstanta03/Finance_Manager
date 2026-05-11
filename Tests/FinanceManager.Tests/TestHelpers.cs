using FinanceManager2._0.Controllers;
using FinanceManager2._0.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace FinanceManager.Tests;

public static class TestHelpers
{
    public static FinanceDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FinanceDbContext(options);
    }

    public static async Task SeedCategoriesAsync(FinanceDbContext context)
    {
        context.Categories.AddRange(
            new Category { Id = 1, Name = "Food", IsExpense = true },
            new Category { Id = 2, Name = "Salary", IsExpense = false },
            new Category { Id = 3, Name = "Transport", IsExpense = true }
        );
        await context.SaveChangesAsync();
    }

    public static ClaimsPrincipal UserPrincipal(string userId = "user1", bool admin = false)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, "user@example.com")
        };

        if (admin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    public static void AttachControllerContext(Controller controller, string userId = "user1", bool admin = false)
    {
        var httpContext = new DefaultHttpContext
        {
            User = UserPrincipal(userId, admin)
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
    }

    public static Mock<UserManager<ApplicationUser>> MockUserManager(ApplicationUser? currentUser = null)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        if (currentUser != null)
            userManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);

        return userManager;
    }

    public static Mock<RoleManager<IdentityRole>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(
            store.Object, null!, null!, null!, null!);
    }

    public static Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager)
    {
        return new Mock<SignInManager<ApplicationUser>>(
            userManager,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
            Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<ApplicationUser>>());
    }
}
