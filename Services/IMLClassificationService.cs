using VERITAS.Models;

namespace VERITAS.Services
{
    public interface IMLClassificationService
    {
        Task<MLClassificationResult> ClassifyAsync(
            string claim);
    }
}