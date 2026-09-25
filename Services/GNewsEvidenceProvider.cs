using System.Text.Json;
using VERITAS.Models;
using VERITAS.Models.GNews;

namespace VERITAS.Services
{
    public class GNewsEvidenceProvider : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GNewsEvidenceProvider(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<NewsArticle>> SearchNewsAsync(string query)
        {
            var apiKey = _configuration["GNews:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return new List<NewsArticle>();
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<NewsArticle>();
            }

            var encodedQuery =
                Uri.EscapeDataString(query.Trim());

            var url =
                $"search?q={encodedQuery}" +
                $"&lang=en" +
                $"&max=10" +
                $"&apikey={apiKey}";

            try
            {
                var response =
                    await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<NewsArticle>();
                }

                var json =
                    await response.Content.ReadAsStringAsync();

                var result =
                    JsonSerializer.Deserialize<GNewsResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result?.Articles == null)
                {
                    return new List<NewsArticle>();
                }

                return result.Articles
                    .Select(article => new NewsArticle
                    {
                        Title = article.Title,
                        Content = article.Content,
                        Source = article.Source?.Name,
                        Url = article.Url,
                        PublishedDate = article.PublishedAt
                    })
                    .ToList();
            }
            catch
            {
                return new List<NewsArticle>();
            }
        }
    }
}