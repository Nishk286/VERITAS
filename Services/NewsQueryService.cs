namespace VERITAS.Services
{
    public class NewsQueryService : INewsQueryService
    {
        private static readonly HashSet<string> StopWords =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "the", "a", "an", "is", "are", "was", "were",
                "be", "been", "being", "has", "have", "had",
                "do", "does", "did", "will", "would", "could",
                "should", "may", "might", "can",
                "this", "that", "these", "those",
                "of", "to", "in", "on", "at", "for", "from",
                "with", "by", "and", "or", "but", "as",
                "it", "its", "they", "their", "them",
                "he", "she", "his", "her", "we", "our",
                "you", "your"
            };

        public string BuildSearchQuery(string claim)
        {
            if (string.IsNullOrWhiteSpace(claim))
            {
                return string.Empty;
            }

            string cleanedClaim = claim
                .Trim()
                .Replace("\"", "")
                .Replace("?", "")
                .Replace("!", "")
                .Replace(".", "")
                .Replace(",", "");

            string[] words = cleanedClaim
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

            var importantWords = words
                .Where(word =>
                    word.Length >= 3 &&
                    !StopWords.Contains(word))
                .Take(12)
                .ToList();

            if (importantWords.Count == 0)
            {
                return cleanedClaim;
            }

            return string.Join(" ", importantWords);
        }
    }
}