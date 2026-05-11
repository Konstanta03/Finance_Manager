using FinanceManager2._0.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceManager2._0.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Compare(
            DateTime? currentStart,
            DateTime? currentEnd,
            DateTime? previousStart,
            DateTime? previousEnd)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "User");

            var today = DateTime.UtcNow;
            var defaultCurrentStart = new DateTime(today.Year, today.Month, 1);
            var defaultCurrentEnd = today.Date;
            var defaultPreviousStart = defaultCurrentStart.AddMonths(-1);
            var defaultPreviousEnd = defaultCurrentStart.AddDays(-1);

            var cStart = currentStart ?? defaultCurrentStart;
            var cEnd = currentEnd ?? defaultCurrentEnd;
            var pStart = previousStart ?? defaultPreviousStart;
            var pEnd = previousEnd ?? defaultPreviousEnd;

            var model = await _statisticsService.CompareAsync(userId, cStart, cEnd, pStart, pEnd);

            ViewBag.CurrentStart = cStart.ToString("yyyy-MM-dd");
            ViewBag.CurrentEnd = cEnd.ToString("yyyy-MM-dd");
            ViewBag.PreviousStart = pStart.ToString("yyyy-MM-dd");
            ViewBag.PreviousEnd = pEnd.ToString("yyyy-MM-dd");

            return View(model);
        }
    }
}
