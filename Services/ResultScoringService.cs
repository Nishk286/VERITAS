using VERITAS.Models;

namespace VERITAS.Services
{
    public class ResultScoringService : IResultScoringService
    {
        public int CalculateConfidence(
            ClaimAnalysisResult result,
            MLClassificationResult? mlResult,
            EvidenceRecord evidence,
            ClaimComparisonResult? comparison,
            SourceAnalysisResult? sourceAnalysis)
        {
            int score = 50;

            // 1. Evidence strength
            if (evidence != null)
            {
                string evidenceType =
                    evidence.EvidenceType?
                        .Trim()
                        .ToLowerInvariant() ?? "";

                if (evidenceType == "supporting" ||
                    evidenceType == "contradicting")
                {
                    score += 25;
                }
            }

            // 2. Machine-learning signal
            if (mlResult != null &&
                mlResult.IsAvailable)
            {
                double mlConfidence =
                    Math.Clamp(
                        mlResult.Confidence,
                        0,
                        100);

                score += (int)Math.Round(
                    mlConfidence * 0.15);
            }

            // 3. Related-news signal
            // News volume is contextual, not proof.
            if (comparison != null)
            {
                int relatedCount =
                    comparison.RelatedArticleCount;

                if (relatedCount >= 5)
                    score += 5;
                else if (relatedCount >= 2)
                    score += 3;
            }

            // 4. Source diversity
            if (sourceAnalysis != null)
            {
                if (sourceAnalysis.DistinctSourceCount >= 5)
                    score += 5;
                else if (sourceAnalysis.DistinctSourceCount >= 2)
                    score += 3;
            }

            // 5. UNCERTAIN results must remain limited
            if (result.Verdict == "UNCERTAIN")
            {
                score = Math.Min(score, 50);
            }

            // Never present the system as absolutely certain.
            return Math.Clamp(score, 0, 95);
        }
    }
}