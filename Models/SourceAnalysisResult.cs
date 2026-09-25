namespace VERITAS.Models
{
    public class SourceAnalysisResult
    {
        public int ArticleCount { get; set; }

        public int DistinctSourceCount { get; set; }

        public int SourcesWithUrl { get; set; }

        public int SecureSourceCount { get; set; }

        public List<string> SourceNames { get; set; } = new();

        public string Summary { get; set; } = "";

        public string Method { get; set; } = "";
    }
}