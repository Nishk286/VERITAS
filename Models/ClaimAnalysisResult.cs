namespace VERITAS.Models
{
    public class ClaimAnalysisResult
    {
        // =====================================================
        // CLAIM
        // =====================================================

        public string Claim { get; set; } = "";


        // =====================================================
        // FINAL VERDICT
        // =====================================================

        public string Verdict { get; set; } = "UNCERTAIN";


        // =====================================================
        // CONFIDENCE
        // =====================================================

        public int Confidence { get; set; }


        // =====================================================
        // ANALYSIS EXPLANATION
        // =====================================================

        public string Explanation { get; set; } = "";


        // =====================================================
        // VERDICT DESCRIPTION
        // =====================================================

        public string VerdictDescription { get; set; } = "";


        // =====================================================
        // EVIDENCE STATUS
        // =====================================================

        public string EvidenceStatus { get; set; } = "";


        // =====================================================
        // EVIDENCE
        // =====================================================

        public string Evidence { get; set; } = "";


        // =====================================================
        // EVIDENCE SOURCE
        // =====================================================

        public string Source { get; set; } = "";


        // =====================================================
        // VERIFICATION METHOD
        // =====================================================

        public string VerificationMethod { get; set; } = "";


        // =====================================================
        // RELATED NEWS INFORMATION
        // =====================================================

        public int RelatedArticleCount { get; set; }

        public int DistinctSourceCount { get; set; }


        // =====================================================
        // ANALYSIS DATE
        // =====================================================
        public string MLLabel { get; set; } = "UNCERTAIN";

        public double MLConfidence { get; set; }

        public string MLModel { get; set; } =
            "ML model not connected";

        public DateTime AnalyzedAt { get; set; } = DateTime.Now;

    }
}