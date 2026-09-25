using VERITAS.Models;

namespace VERITAS.Services
{
    public class EvidenceAnalysisService : IEvidenceAnalysisService
    {
        public ClaimAnalysisResult AnalyzeEvidence(
            ClaimAnalysisResult analysis,
            List<NewsArticle> relatedNews)
        {
            // =================================================
            // NO RELATED NEWS
            // =================================================

            if (relatedNews == null || relatedNews.Count == 0)
            {
                analysis.RelatedArticleCount = 0;

                analysis.DistinctSourceCount = 0;

                // Do not increase confidence simply because
                // news retrieval was attempted.

                if (analysis.Verdict == "UNCERTAIN")
                {
                    analysis.EvidenceStatus = "Insufficient";

                    analysis.Explanation =
                        "VERITAS did not retrieve sufficient " +
                        "related reporting to independently verify " +
                        "this claim.";

                    analysis.VerificationMethod =
                        "Preliminary claim analysis; reliable " +
                        "supporting or contradicting evidence was " +
                        "not available.";

                    analysis.Confidence =
                        Math.Min(analysis.Confidence, 50);
                }

                return analysis;
            }


            // =================================================
            // COUNT RELATED ARTICLES
            // =================================================

            int articleCount =
                relatedNews.Count;

            analysis.RelatedArticleCount =
                articleCount;


            // =================================================
            // COUNT DISTINCT SOURCES
            // =================================================

            int uniqueSources =
                relatedNews
                    .Where(article =>
                        !string.IsNullOrWhiteSpace(
                            article.Source))
                    .Select(article =>
                        article.Source!
                            .Trim()
                            .ToLowerInvariant())
                    .Distinct()
                    .Count();

            analysis.DistinctSourceCount =
                uniqueSources;


            // =================================================
            // IMPORTANT:
            // NEWS CONTEXT DOES NOT AUTOMATICALLY VERIFY CLAIMS
            // =================================================

            if (analysis.Verdict == "UNCERTAIN")
            {
                analysis.EvidenceStatus =
                    "Context available";

                analysis.Confidence =
                    Math.Min(
                        analysis.Confidence,
                        50);

                analysis.Explanation =
                    $"VERITAS retrieved {articleCount} " +
                    $"related news article(s) from " +
                    $"{uniqueSources} distinct source(s). " +
                    "These reports provide contextual information, " +
                    "but their presence alone is not sufficient " +
                    "to establish the claim as true or false.";

                analysis.VerificationMethod =
                    "Preliminary claim analysis with live-news " +
                    "context. Retrieved reporting is treated as " +
                    "contextual information rather than automatic proof.";

                analysis.Source =
                    "GNews live news retrieval";
            }

            return analysis;
        }
    }
}