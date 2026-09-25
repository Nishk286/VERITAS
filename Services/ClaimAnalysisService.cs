using VERITAS.Models;

namespace VERITAS.Services
{
    public class ClaimAnalysisService : IDetectionService
    {
        public ClaimAnalysisResult Detect(string claim)
        {
            // Check whether the user entered a claim
            if (string.IsNullOrWhiteSpace(claim))
            {
                return new ClaimAnalysisResult
                {
                    Claim = claim,
                    Verdict = "UNCERTAIN",
                    Confidence = 0,

                    Explanation =
                        "No claim was provided for analysis.",

                    VerdictDescription =
                        "A conclusion cannot be reached without a claim.",

                    EvidenceStatus =
                        "Insufficient",

                    Evidence =
                        "No evidence can be evaluated because no claim was provided.",

                    Source =
                        "VERITAS",

                    VerificationMethod =
                        "Preliminary analysis; no claim was available for verification."
                };
            }

            // Convert the claim to lowercase
            // so that matching is not affected by capital letters.
            string text = claim.ToLowerInvariant();

            // ==========================================
            // FALSE CLAIM
            // ==========================================

            if (text.Contains("earth is flat"))
            {
                return new ClaimAnalysisResult
                {
                    Claim = claim,

                    Verdict = "FALSE",

                    Confidence = 98,

                    Explanation =
                        "The claim conflicts with established scientific evidence.",

                    VerdictDescription =
                        "Available evidence contradicts this claim.",

                    EvidenceStatus =
                        "Contradicting",

                    Evidence =
                        "Scientific observations, satellite measurements, and astronomical evidence contradict the claim that Earth is flat.",

                    Source =
                        "Established scientific evidence",

                    VerificationMethod =
                        "Rule-based preliminary analysis with contradicting evidence.",

                    AnalyzedAt = DateTime.Now
                };
            }

            // ==========================================
            // TRUE CLAIM
            // ==========================================

            if (text.Contains("water freezes at 0"))
            {
                return new ClaimAnalysisResult
                {
                    Claim = claim,

                    Verdict = "TRUE",

                    Confidence = 98,

                    Explanation =
                        "The claim is consistent with established scientific knowledge under standard atmospheric pressure.",

                    VerdictDescription =
                        "Available evidence supports this claim.",

                    EvidenceStatus =
                        "Supporting",

                    Evidence =
                        "Pure water freezes at approximately 0°C under standard atmospheric pressure.",

                    Source =
                        "Established scientific knowledge",

                    VerificationMethod =
                        "Rule-based preliminary analysis with supporting evidence.",

                    AnalyzedAt = DateTime.Now
                };
            }

            // ==========================================
            // UNCERTAIN CLAIM
            // ==========================================

            return new ClaimAnalysisResult
            {
                Claim = claim,

                Verdict = "UNCERTAIN",

                Confidence = 50,

                Explanation =
                    "VERITAS could not determine the claim from its current analysis data.",

                VerdictDescription =
                    "There is not enough reliable evidence to reach a confident conclusion.",

                EvidenceStatus =
                    "Insufficient",

                Evidence =
                    "VERITAS currently does not have sufficient evidence to independently verify this claim.",

                Source =
                    "VERITAS preliminary analysis",

                VerificationMethod =
                    "Preliminary analysis; reliable evidence was insufficient.",

                AnalyzedAt = DateTime.Now
            };
        }

        // This method is kept for compatibility
        // with the earlier Analyze() implementation.
        public ClaimAnalysisResult Analyze(string claim)
        {
            return Detect(claim);
        }
    }
}