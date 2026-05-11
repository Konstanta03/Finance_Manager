using Microsoft.AspNetCore.Identity;
using FinanceManager2._0.Models;

namespace FinanceManager2._0.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ✅ Створюємо ролі
            string[] roles = new[] { "Guest", "User", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ✅ Створюємо базового адміна
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // ✅ Базовий гість
            var guestUser = await userManager.FindByNameAsync("guest");
            if (guestUser == null)
            {
                guestUser = new ApplicationUser
                {
                    UserName = "guest",
                    Email = "guest@example.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                await userManager.CreateAsync(guestUser, "Guest123!");
                await userManager.AddToRoleAsync(guestUser, "Guest");
            }
        }
    }
}