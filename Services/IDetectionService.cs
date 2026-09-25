using VERITAS.Models;

namespace VERITAS.Services
{
    public interface IDetectionService
    {
        ClaimAnalysisResult Detect(string claim);
    }
}