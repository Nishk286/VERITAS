namespace VERITAS.Models
{
    public class MLClassificationResult
    {
        public string Label { get; set; } = "UNCERTAIN";

        public double Confidence { get; set; }

        public string Explanation { get; set; } =
            "No machine-learning prediction is currently available.";

        public bool IsAvailable { get; set; } = false;

        public string ModelName { get; set; } =
            "ML model not connected";
    }
}