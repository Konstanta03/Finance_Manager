using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceManager2._0.Models;
using Microsoft.AspNetCore.Identity;

namespace FinanceManager2._0.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly FinanceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SettingsController(FinanceDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "User");

            var settings = await _context.Settings.FirstOrDefaultAsync(s => s.UserId == user.Id)
                ?? new Settings { UserId = user.Id, Currency = "UAH", Language = "uk", Theme = "light", SaveSettings = true };

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Currency,Language,Theme,SaveSettings,ExportData")] Settings model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "User");

            ModelState.Remove(nameof(Settings.User));
            ModelState.Remove(nameof(Settings.UserId));

            if (!ModelState.IsValid) return View(model);

            var existing = await _context.Settings.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (existing == null)
            {
                existing = new Settings { UserId = user.Id };
                _context.Settings.Add(existing);
            }

            existing.Currency = model.Currency ?? "UAH";
            existing.Language = model.Language ?? "uk";
            existing.Theme = model.Theme ?? "light";
            existing.SaveSettings = model.SaveSettings;
            existing.ExportData = model.ExportData ?? "";

            await _context.SaveChangesAsync();
            TempData["Success"] = "Налаштування збережено";

            return RedirectToAction(nameof(Index));
        }
    }
}
