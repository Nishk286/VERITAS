using Microsoft.EntityFrameworkCore;
using VERITAS.Data;
using VERITAS.Models;

namespace VERITAS.Services
{
    public class DatabaseEvidenceProvider : IEvidenceProvider
    {
        private readonly VeritasDbContext _dbContext;
        private readonly IDetectionService _detectionService;

        public DatabaseEvidenceProvider(
            VeritasDbContext dbContext,
            IDetectionService detectionService)
        {
            _dbContext = dbContext;
            _detectionService = detectionService;
        }

        public async Task<EvidenceRecord> GetEvidenceAsync(string claim)
        {
            if (string.IsNullOrWhiteSpace(claim))
            {
                return new EvidenceRecord
                {
                    Claim = claim,
                    Evidence =
                        "No claim was provided for verification.",
                    Source = "VERITAS",
                    EvidenceType = "Insufficient"
                };
            }

            claim = claim.Trim();

            // -------------------------------------------------
            // STEP 1: Check the VERITAS evidence database
            // -------------------------------------------------

            var evidence = await _dbContext.EvidenceRecords
                .FirstOrDefaultAsync(e =>
                    e.Claim.ToLower() == claim.ToLower());

            if (evidence != null)
            {
                return evidence;
            }

            // -------------------------------------------------
            // STEP 2: Use the existing rule-based analysis
            // as a fallback evidence source.
            // -------------------------------------------------

            var analysis =
                _detectionService.Detect(claim);

            if (analysis != null)
            {
                string evidenceStatus =
                    analysis.EvidenceStatus?
                        .Trim()
                        .ToLowerInvariant() ?? "";

                if (evidenceStatus == "supporting")
                {
                    return new EvidenceRecord
                    {
                        Claim = claim,
                        Evidence = analysis.Evidence,
                        Source = analysis.Source,
                        EvidenceType = "Supporting"
                    };
                }

                if (evidenceStatus == "contradicting")
                {
                    return new EvidenceRecord
                    {
                        Claim = claim,
                        Evidence = analysis.Evidence,
                        Source = analysis.Source,
                        EvidenceType = "Contradicting"
                    };
                }
            }

            // -------------------------------------------------
            // STEP 3: No reliable evidence available
            // -------------------------------------------------

            return new EvidenceRecord
            {
                Claim = claim,

                Evidence =
                    "No matching evidence was found in the " +
                    "VERITAS evidence database, and the current " +
                    "rule-based evidence layer could not establish " +
                    "the claim.",

                Source =
                    "VERITAS Evidence Database",

                EvidenceType =
                    "Insufficient"
            };
        }
    }
}