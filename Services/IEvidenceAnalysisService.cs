using VERITAS.Models;

namespace VERITAS.Services
{
    public interface IEvidenceAnalysisService
    {
        ClaimAnalysisResult AnalyzeEvidence(
            ClaimAnalysisResult analysis,
            List<NewsArticle> relatedNews);
    }
}