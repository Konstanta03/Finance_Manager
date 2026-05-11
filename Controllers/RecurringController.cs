using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceManager2._0.Models;
using System.Security.Claims;

namespace FinanceManager2._0.Controllers
{
    [Authorize]
    public class RecurringController : Controller
    {
        private readonly FinanceDbContext _context;

        public RecurringController(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recurring = await _context.RecurringPayments
                .Include(r => r.Category)
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.StartDate)
                .ToListAsync();

            return View(recurring);
        }

        public IActionResult Create()
        {
            LoadCategories();
            return View(new RecurringPayment { StartDate = DateTime.UtcNow, IsActive = true, Frequency = "Monthly" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Amount,CategoryId,StartDate,Frequency,IsActive")] RecurringPayment model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "User");

            model.UserId = userId;
            model.StartDate = DateTime.SpecifyKind(model.StartDate == default ? DateTime.UtcNow : model.StartDate, DateTimeKind.Utc);

            ModelState.Remove(nameof(RecurringPayment.User));
            ModelState.Remove(nameof(RecurringPayment.UserId));
            ModelState.Remove(nameof(RecurringPayment.Category));

            if (!_context.Categories.Any(c => c.Id == model.CategoryId))
                ModelState.AddModelError(nameof(RecurringPayment.CategoryId), "Оберіть категорію.");

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(model);
            }

            _context.RecurringPayments.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Регулярний платіж додано";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recurring = await _context.RecurringPayments.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (recurring == null) return NotFound();

            LoadCategories();
            return View(recurring);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Amount,CategoryId,StartDate,Frequency,IsActive")] RecurringPayment form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existing = await _context.RecurringPayments.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (existing == null) return NotFound();

            ModelState.Remove(nameof(RecurringPayment.User));
            ModelState.Remove(nameof(RecurringPayment.UserId));
            ModelState.Remove(nameof(RecurringPayment.Category));

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(form);
            }

            existing.Name = form.Name;
            existing.Amount = form.Amount;
            existing.CategoryId = form.CategoryId;
            existing.StartDate = DateTime.SpecifyKind(form.StartDate == default ? DateTime.UtcNow : form.StartDate, DateTimeKind.Utc);
            existing.Frequency = form.Frequency;
            existing.IsActive = form.IsActive;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Регулярний платіж оновлено";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recurring = await _context.RecurringPayments.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (recurring != null)
            {
                _context.RecurringPayments.Remove(recurring);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Регулярний платіж видалено";
            }
            return RedirectToAction(nameof(Index));
        }

        private void LoadCategories()
        {
            ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
        }
    }
}
