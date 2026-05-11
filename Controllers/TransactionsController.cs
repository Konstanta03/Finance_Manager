using FinanceManager2._0.Models;
using FinanceManager2._0.Services;
using FinanceManager2._0.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager2._0.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly FinanceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransactionService _transactionService;

        public TransactionsController(
            FinanceDbContext context,
            UserManager<ApplicationUser> userManager,
            ITransactionService transactionService)
        {
            _context = context;
            _userManager = userManager;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(TransactionFilterViewModel filter)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "User");

            var transactionsQuery = _transactionService.BuildQuery(user.Id, User.IsInRole("Admin"), filter);

            ViewBag.MinAmount = filter.MinAmount;
            ViewBag.MaxAmount = filter.MaxAmount;
            ViewBag.StartDate = filter.StartDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = filter.EndDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedCategoryId = filter.CategoryId;

            LoadDropdowns(filter.CategoryId);
            return View(await transactionsQuery.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            if (!await _context.Categories.AnyAsync())
                TempData["Warning"] = "Спочатку додайте хоча б одну категорію.";

            LoadDropdowns();
            return View(new Transaction { Date = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Amount,Date,Note")] Transaction transaction)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "User");

            PrepareTransactionModelState();
            ValidateCategory(transaction.CategoryId);

            if (!ModelState.IsValid)
            {
                LoadDropdowns(transaction.CategoryId);
                return View(transaction);
            }

            await _transactionService.CreateAsync(transaction, user.Id);
            TempData["Success"] = "Транзакція додана";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "User");

            var transaction = await _transactionService.GetForEditAsync(id.Value, currentUser.Id, User.IsInRole("Admin"));
            if (transaction == null) return NotFound();

            LoadDropdowns(transaction.CategoryId);
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Amount,Date,Note")] Transaction form)
        {
            if (id != form.Id) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "User");

            PrepareTransactionModelState();
            ValidateCategory(form.CategoryId);

            if (!ModelState.IsValid)
            {
                LoadDropdowns(form.CategoryId);
                return View(form);
            }

            var updated = await _transactionService.UpdateAsync(id, form, currentUser.Id, User.IsInRole("Admin"));
            if (!updated) return NotFound();

            TempData["Success"] = "Транзакція оновлена";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "User");

            var deleted = await _transactionService.DeleteAsync(id, currentUser.Id, User.IsInRole("Admin"));
            if (deleted) TempData["Success"] = "Транзакцію видалено";

            return RedirectToAction(nameof(Index));
        }

        private void PrepareTransactionModelState()
        {
            ModelState.Remove(nameof(Transaction.User));
            ModelState.Remove(nameof(Transaction.UserId));
            ModelState.Remove(nameof(Transaction.Category));
        }

        private void ValidateCategory(int categoryId)
        {
            if (!_context.Categories.Any(c => c.Id == categoryId))
                ModelState.AddModelError(nameof(Transaction.CategoryId), "Оберіть категорію.");
        }

        private void LoadDropdowns(int? selectedCategoryId = null)
        {
            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.Name).ToList(),
                "Id",
                "Name",
                selectedCategoryId
            );
        }
    }
}
