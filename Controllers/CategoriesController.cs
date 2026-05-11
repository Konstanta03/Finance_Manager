using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceManager2._0.Models;

namespace FinanceManager2._0.Controllers
{
    [Authorize] // 🔹 Доступ лише для авторизованих
    public class CategoriesController : Controller
    {
        private readonly FinanceDbContext _context;

        public CategoriesController(FinanceDbContext context)
        {
            _context = context;
        }

        // ===================== INDEX =====================
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();

            // 🔹 Перевірка null, щоб уникнути NullReferenceException
            if (categories == null)
                categories = new List<Category>();

            return View(categories);
        }

        // ===================== CREATE =====================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Категорія додана";
            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT =====================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(category);

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Категорія оновлена";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExists(category.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Категорія видалена";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== HELPERS =====================
        private async Task<bool> CategoryExists(int id)
        {
            return await _context.Categories.AnyAsync(e => e.Id == id);
        }
    }
}