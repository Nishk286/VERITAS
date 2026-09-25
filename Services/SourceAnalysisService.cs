using VERITAS.Models;

namespace VERITAS.Services
{
    public class SourceAnalysisService : ISourceAnalysisService
    {
        public SourceAnalysisResult Analyze(
            List<NewsArticle> articles)
        {
            var result = new SourceAnalysisResult
            {
                ArticleCount =
                    articles?.Count ?? 0
            };

            if (articles == null ||
                articles.Count == 0)
            {
                result.Summary =
                    "No article sources were available " +
                    "for analysis.";

                result.Method =
                    "Source metadata analysis.";

                return result;
            }

            var sourceNames =
                articles
                    .Where(article =>
                        !string.IsNullOrWhiteSpace(
                            article.Source))
                    .Select(article =>
                        article.Source!.Trim())
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name)
                    .ToList();

            result.SourceNames =
                sourceNames;

            result.DistinctSourceCount =
                sourceNames.Count;

            result.SourcesWithUrl =
                articles.Count(article =>
                    !string.IsNullOrWhiteSpace(
                        article.Url));

            result.SecureSourceCount =
                articles.Count(article =>
                    IsSecureUrl(article.Url));

            result.Summary =
                BuildSummary(result);

            result.Method =
                "Source metadata analysis using source names, " +
                "article URLs, HTTPS availability, and source diversity.";

            return result;
        }

        private bool IsSecureUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            if (!Uri.TryCreate(
                    url,
                    UriKind.Absolute,
                    out Uri? uri))
            {
                return false;
            }

            return uri.Scheme ==
                   Uri.UriSchemeHttps;
        }

        private string BuildSummary(
            SourceAnalysisResult result)
        {
            if (result.ArticleCount == 0)
            {
                return
                    "No article sources were available " +
                    "for analysis.";
            }

            if (result.DistinctSourceCount == 0)
            {
                return
                    $"{result.ArticleCount} article(s) were retrieved, " +
                    "but source names were not available.";
            }

            return
                $"{result.ArticleCount} article(s) were retrieved " +
                $"from {result.DistinctSourceCount} distinct " +
                $"source(s). " +
                $"{result.SourcesWithUrl} article(s) included " +
                $"a source URL, and {result.SecureSourceCount} " +
                $"used HTTPS.";
        }
    }
}