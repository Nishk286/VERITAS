namespace VERITAS.Models
{
    public class ClaimComparisonResult
    {
        public string Claim { get; set; } = "";

        public int RelatedArticleCount { get; set; }

        public int DistinctSourceCount { get; set; }

        public int SupportingSignals { get; set; }

        public int ContradictingSignals { get; set; }

        public int NeutralSignals { get; set; }

        public string Summary { get; set; } = "";

        public string Method { get; set; } = "";
    }
}