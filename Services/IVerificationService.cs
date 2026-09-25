using VERITAS.Models;

namespace VERITAS.Services
{
    public interface IVerificationService
    {
        ClaimAnalysisResult Verify(
            string claim,
            ClaimAnalysisResult analysis,
            EvidenceRecord evidence);
    }
}