using VERITAS.Models;

namespace VERITAS.Services
{
    public interface IEvidenceProvider
    {
        Task<EvidenceRecord> GetEvidenceAsync(string claim);
    }
}