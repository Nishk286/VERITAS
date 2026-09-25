using VERITAS.Models;

namespace VERITAS.Services
{
    public interface ISourceAnalysisService
    {
        SourceAnalysisResult Analyze(
            List<NewsArticle> articles);
    }
}