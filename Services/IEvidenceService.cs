using VERITAS.Models;

namespace VERITAS.Services
{
    public interface IEvidenceService
    {
        EvidenceRecord GetEvidence(string claim);
    }
}