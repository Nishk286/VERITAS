using VERITAS.Models;

namespace VERITAS.Services
{
    public class VerificationService : IVerificationService
    {
        public ClaimAnalysisResult Verify(
            string claim,
            ClaimAnalysisResult analysis,
            EvidenceRecord evidence)
        {
            // STEP 1: Validate claim
            if (string.IsNullOrWhiteSpace(claim))
            {
                return BuildUncertainResult(
                    analysis,
                    0,
                    "A reliable conclusion cannot be reached because no claim was provided.",
                    "Verification could not be performed without a claim.");
            }

            // STEP 2: Validate evidence
            if (evidence == null ||
                string.IsNullOrWhiteSpace(evidence.Evidence))
            {
                return BuildUncertainResult(
                    analysis,
                    Math.Min(analysis.Confidence, 50),
                    "There is not enough reliable evidence to reach a confident conclusion.",
                    "Evidence-based verification; reliable evidence was not available.");
            }

            string evidenceType =
                evidence.EvidenceType?
                    .Trim()
                    .ToLowerInvariant() ?? "unknown";

            // STEP 3: Apply evidence
            switch (evidenceType)
            {
                case "supporting":

                    analysis.Verdict = "TRUE";

                    analysis.Confidence =
                        CalculateEvidenceConfidence(
                            analysis.Confidence);

                    analysis.EvidenceStatus =
                        "Supporting";

                    analysis.VerdictDescription =
                        "Available evidence supports this claim.";

                    analysis.VerificationMethod =
                        "Evidence-based verification using supporting evidence.";

                    break;

                case "contradicting":

                    analysis.Verdict = "FALSE";

                    analysis.Confidence =
                        CalculateEvidenceConfidence(
                            analysis.Confidence);

                    analysis.EvidenceStatus =
                        "Contradicting";

                    analysis.VerdictDescription =
                        "Available evidence contradicts this claim.";

                    analysis.VerificationMethod =
                        "Evidence-based verification using contradicting evidence.";

                    break;

                case "insufficient":

                    return BuildUncertainResult(
                        analysis,
                        Math.Min(analysis.Confidence, 50),
                        "There is not enough reliable evidence to reach a confident conclusion.",
                        "Evidence-based verification; available evidence was insufficient.");

                default:

                    return BuildUncertainResult(
                        analysis,
                        Math.Min(analysis.Confidence, 50),
                        "The available evidence could not support a reliable conclusion.",
                        "Evidence-based verification; the evidence type could not be reliably classified.");
            }

            // STEP 4: Store evidence details
            analysis.Evidence =
                evidence.Evidence;

            analysis.Source =
                evidence.Source;

            return analysis;
        }

        private int CalculateEvidenceConfidence(
            int existingConfidence)
        {
            /*
             * Evidence is stronger than a preliminary rule-based
             * classification, but we avoid presenting the result
             * as absolute certainty.
             */

            int confidence =
                Math.Max(existingConfidence, 80);

            return Math.Min(confidence, 95);
        }

        private ClaimAnalysisResult BuildUncertainResult(
            ClaimAnalysisResult analysis,
            int confidence,
            string description,
            string method)
        {
            analysis.Verdict =
                "UNCERTAIN";

            analysis.Confidence =
                Math.Clamp(confidence, 0, 50);

            analysis.EvidenceStatus =
                "Insufficient";

            analysis.VerdictDescription =
                description;

            analysis.VerificationMethod =
                method;

            return analysis;
        }
    }
}