using VERITAS.Models;

namespace VERITAS.Services
{
    public class EvidenceService : IEvidenceService
    {
        public EvidenceRecord GetEvidence(string claim)
        {
            if (string.IsNullOrWhiteSpace(claim))
            {
                return new EvidenceRecord
                {
                    Claim = claim,
                    Evidence =
                        "No claim was provided.",

                    Source =
                        "VERITAS",

                    EvidenceType =
                        "Insufficient"
                };
            }

            string text = claim.ToLowerInvariant();

            // ==========================================
            // EARTH IS FLAT
            // ==========================================

            if (text.Contains("earth is flat"))
            {
                return new EvidenceRecord
                {
                    Claim = claim,

                    Evidence =
                        "Scientific observations, satellite measurements, and astronomical evidence contradict the claim that Earth is flat.",

                    Source =
                        "Established scientific evidence",

                    EvidenceType =
                        "Contradicting"
                };
            }

            // ==========================================
            // WATER FREEZES AT 0
            // ==========================================

            if (text.Contains("water freezes at 0"))
            {
                return new EvidenceRecord
                {
                    Claim = claim,

                    Evidence =
                        "Pure water freezes at approximately 0°C under standard atmospheric pressure.",

                    Source =
                        "Established scientific knowledge",

                    EvidenceType =
                        "Supporting"
                };
            }

            // ==========================================
            // UNKNOWN CLAIM
            // ==========================================

            return new EvidenceRecord
            {
                Claim = claim,

                Evidence =
                    "VERITAS currently does not have sufficient evidence to independently verify this claim.",

                Source =
                    "VERITAS preliminary analysis",

                EvidenceType =
                    "Insufficient"
            };
        }
    }
}