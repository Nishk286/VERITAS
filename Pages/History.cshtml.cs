using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VERITAS.Data;
using VERITAS.Models;

namespace VERITAS.Pages
{
    public class HistoryModel : PageModel
    {
        private readonly VeritasDbContext _dbContext;

        public HistoryModel(VeritasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // =========================================================
        // HISTORY RESULTS
        // =========================================================

        public List<DetectionResult> Results { get; private set; } = new();


        // =========================================================
        // PAGINATION
        // =========================================================

        public int CurrentPage { get; private set; } = 1;

        public int TotalPages { get; private set; } = 1;

        public int PageSize { get; } = 10;


        // =========================================================
        // GET
        // =========================================================

        public async Task OnGetAsync(int page = 1)
        {
            int totalRecords =
                await _dbContext.DetectionResults
                    .CountAsync();

            TotalPages =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        totalRecords / (double)PageSize));

            CurrentPage =
                Math.Clamp(
                    page,
                    1,
                    TotalPages);

            Results =
                await _dbContext.DetectionResults
                    .AsNoTracking()
                    .OrderByDescending(
                        record => record.CheckedDate)
                    .Skip(
                        (CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
        }
    }
}