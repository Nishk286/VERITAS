using VERITAS.Models;

namespace VERITAS.Services
{
    public interface IResultScoringService
    {
        int CalculateConfidence(
            ClaimAnalysisResult result,
            MLClassificationResult? mlResult,
            EvidenceRecord evidence,
            ClaimComparisonResult? comparison,
            SourceAnalysisResult? sourceAnalysis);
    }
}