using Microsoft.AspNetCore.Mvc.RazorPages;
using VERITAS.Models;
using VERITAS.Services;

namespace VERITAS.Pages
{
    public class LatestNewsModel : PageModel
    {
        private readonly INewsService _newsService;

        public LatestNewsModel(INewsService newsService)
        {
            _newsService = newsService;
        }

        public List<NewsArticle> Articles { get; set; } = new();

        public bool HasError { get; private set; }

        public string? ErrorMessage { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                Articles =
                    await _newsService.SearchNewsAsync("latest news");

                if (Articles == null)
                {
                    Articles = new List<NewsArticle>();
                }
            }
            catch
            {
                HasError = true;

                ErrorMessage =
                    "Latest news is temporarily unavailable. Please try again later.";

                Articles = new List<NewsArticle>();
            }
        }
    }
}