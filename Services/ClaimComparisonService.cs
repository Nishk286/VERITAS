using VERITAS.Models;

namespace VERITAS.Services
{
    public class ClaimComparisonService : IClaimComparisonService
    {
        private static readonly HashSet<string> StopWords =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "the", "a", "an", "is", "are", "was", "were",
                "be", "been", "being", "has", "have", "had",
                "do", "does", "did", "will", "would", "could",
                "should", "may", "might", "can", "cannot",
                "this", "that", "these", "those",
                "of", "to", "in", "on", "at", "for", "from",
                "with", "by", "and", "or", "but", "as",
                "it", "its", "they", "their", "them",
                "he", "she", "his", "her", "we", "our",
                "you", "your", "who", "what", "when", "where",
                "why", "how", "about", "into", "than", "then"
            };

        private static readonly string[] ContradictionPhrases =
        {
            "false claim",
            "false report",
            "false reports",
            "fact check",
            "fact-check",
            "debunked",
            "debunks",
            "debunk",
            "denied",
            "denies",
            "denied the claim",
            "rejected the claim",
            "not true",
            "untrue",
            "incorrect",
            "misleading",
            "fabricated",
            "hoax"
        };

        private static readonly string[] SupportingPhrases =
        {
            "confirmed",
            "confirms",
            "confirmed that",
            "officially confirmed",
            "according to officials",
            "official statement",
            "verified",
            "evidence shows",
            "research shows",
            "study found",
            "data shows",
            "data indicate",
            "reported by"
        };

        public ClaimComparisonResult Compare(
            string claim,
            List<NewsArticle> relatedNews)
        {
            var result = new ClaimComparisonResult
            {
                Claim = claim ?? "",
                RelatedArticleCount =
                    relatedNews?.Count ?? 0
            };

            if (string.IsNullOrWhiteSpace(claim))
            {
                result.Summary =
                    "No claim was available for comparison.";

                result.Method =
                    "Text relevance and evidence-signal analysis.";

                return result;
            }

            if (relatedNews == null ||
                relatedNews.Count == 0)
            {
                result.Summary =
                    "No related news reporting was available " +
                    "for comparison.";

                result.Method =
                    "Text relevance and evidence-signal analysis; " +
                    "no articles were available.";

                return result;
            }

            result.DistinctSourceCount =
                relatedNews
                    .Where(article =>
                        !string.IsNullOrWhiteSpace(article.Source))
                    .Select(article =>
                        article.Source!
                            .Trim()
                            .ToLowerInvariant())
                    .Distinct()
                    .Count();

            List<string> claimWords =
                ExtractKeywords(claim);

            if (claimWords.Count == 0)
            {
                result.NeutralSignals =
                    relatedNews.Count;

                result.Summary =
                    "The claim did not contain enough meaningful " +
                    "terms for reliable text comparison.";

                result.Method =
                    "Text relevance and evidence-signal analysis.";

                return result;
            }

            foreach (var article in relatedNews)
            {
                string title =
                    article.Title ?? "";

                string content =
                    article.Content ?? "";

                string articleText =
                    $"{title} {content}";

                List<string> articleWords =
                    ExtractKeywords(articleText);

                int keywordOverlap =
                    claimWords
                        .Intersect(
                            articleWords,
                            StringComparer.OrdinalIgnoreCase)
                        .Count();

                double overlapRatio =
                    claimWords.Count == 0
                        ? 0
                        : (double)keywordOverlap /
                          claimWords.Count;

                bool hasSupportingLanguage =
                    ContainsAnyPhrase(
                        articleText,
                        SupportingPhrases);

                bool hasContradictionLanguage =
                    ContainsAnyPhrase(
                        articleText,
                        ContradictionPhrases);

                bool titleIsRelevant =
                    CalculateTitleOverlap(
                        claimWords,
                        title);

                /*
                 * Strong relevance:
                 * - at least 3 shared meaningful terms
                 * - or strong title overlap
                 */
                bool relevantArticle =
                    keywordOverlap >= 3 ||
                    titleIsRelevant;

                if (!relevantArticle)
                {
                    result.NeutralSignals++;
                    continue;
                }

                /*
                 * Contradiction signals are counted only when
                 * the article is relevant AND contains explicit
                 * contradiction language.
                 */
                if (hasContradictionLanguage)
                {
                    result.ContradictingSignals++;
                    continue;
                }

                /*
                 * Supporting signals require relevant content
                 * and explicit supporting language.
                 */
                if (hasSupportingLanguage &&
                    (keywordOverlap >= 3 ||
                     overlapRatio >= 0.35))
                {
                    result.SupportingSignals++;
                    continue;
                }

                /*
                 * Relevant reporting without explicit support
                 * or contradiction remains contextual/neutral.
                 */
                result.NeutralSignals++;
            }

            result.Summary =
                BuildSummary(result);

            result.Method =
                "Claim-to-news text relevance analysis using " +
                "keyword overlap, title relevance, and explicit " +
                "supporting or contradiction language. " +
                "Signals are contextual and are not treated as " +
                "independent proof.";

            return result;
        }

        private List<string> ExtractKeywords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            string cleaned =
                new string(
                    text
                        .ToLowerInvariant()
                        .Select(character =>
                            char.IsLetterOrDigit(character)
                                ? character
                                : ' ')
                        .ToArray());

            return cleaned
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(word =>
                    word.Length >= 3 &&
                    !StopWords.Contains(word))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private bool ContainsAnyPhrase(
            string text,
            string[] phrases)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            string normalized =
                text.ToLowerInvariant();

            return phrases.Any(
                phrase =>
                    normalized.Contains(
                        phrase,
                        StringComparison.OrdinalIgnoreCase));
        }

        private bool CalculateTitleOverlap(
            List<string> claimWords,
            string title)
        {
            if (string.IsNullOrWhiteSpace(title) ||
                claimWords.Count == 0)
            {
                return false;
            }

            List<string> titleWords =
                ExtractKeywords(title);

            int overlap =
                claimWords
                    .Intersect(
                        titleWords,
                        StringComparer.OrdinalIgnoreCase)
                    .Count();

            double ratio =
                (double)overlap /
                claimWords.Count;

            return overlap >= 2 &&
                   ratio >= 0.30;
        }

        private string BuildSummary(
            ClaimComparisonResult result)
        {
            if (result.RelatedArticleCount == 0)
            {
                return
                    "No related news reporting was available " +
                    "for comparison.";
            }

            if (result.SupportingSignals == 0 &&
                result.ContradictingSignals == 0)
            {
                return
                    $"VERITAS reviewed {result.RelatedArticleCount} " +
                    $"related article(s) from " +
                    $"{result.DistinctSourceCount} distinct source(s). " +
                    "The retrieved reporting provides contextual " +
                    "information, but no strong supporting or " +
                    "contradicting signal was detected.";
            }

            return
                $"VERITAS reviewed {result.RelatedArticleCount} " +
                $"related article(s) from " +
                $"{result.DistinctSourceCount} distinct source(s). " +
                $"The comparison detected " +
                $"{result.SupportingSignals} supporting signal(s), " +
                $"{result.ContradictingSignals} contradicting " +
                $"signal(s), and " +
                $"{result.NeutralSignals} neutral/contextual " +
                "article(s). These signals require further " +
                "verification and are not treated as proof.";
        }
    }
}