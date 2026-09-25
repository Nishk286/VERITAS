using VERITAS.Models;

namespace VERITAS.Services
{
    public interface INewsService
    {
        Task<List<NewsArticle>> SearchNewsAsync(string query);
    }
}