using VERITAS.Models;

namespace VERITAS.Services
{
    public interface IClaimComparisonService
    {
        ClaimComparisonResult Compare(
            string claim,
            List<NewsArticle> relatedNews);
    }
}