using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VERITAS.Data;
using VERITAS.Models;
using VERITAS.Services;

namespace VERITAS.Pages
{
    public class AnalyzeModel : PageModel
    {
        private readonly IDetectionService _detectionService;
        private readonly IMLClassificationService _mlClassificationService;
        private readonly IEvidenceProvider _evidenceProvider;
        private readonly IVerificationService _verificationService;
        private readonly INewsService _newsService;
        private readonly IEvidenceAnalysisService _evidenceAnalysisService;
        private readonly INewsQueryService _newsQueryService;
        private readonly IClaimComparisonService _claimComparisonService;
        private readonly ISourceAnalysisService _sourceAnalysisService;
        private readonly IResultScoringService _resultScoringService;
        private readonly VeritasDbContext _dbContext;

        public AnalyzeModel(
            IDetectionService detectionService,
            IMLClassificationService mlClassificationService,
            IEvidenceProvider evidenceProvider,
            IVerificationService verificationService,
            INewsService newsService,
            IEvidenceAnalysisService evidenceAnalysisService,
            INewsQueryService newsQueryService,
            IClaimComparisonService claimComparisonService,
            ISourceAnalysisService sourceAnalysisService,
            IResultScoringService resultScoringService,
            VeritasDbContext dbContext)
        {
            _detectionService = detectionService;
            _mlClassificationService = mlClassificationService;
            _evidenceProvider = evidenceProvider;
            _verificationService = verificationService;
            _newsService = newsService;
            _evidenceAnalysisService = evidenceAnalysisService;
            _newsQueryService = newsQueryService;
            _claimComparisonService = claimComparisonService;
            _sourceAnalysisService = sourceAnalysisService;
            _resultScoringService = resultScoringService;
            _dbContext = dbContext;
        }

        // ============================================================
        // FORM INPUT
        // ============================================================

        [BindProperty]
        [StringLength(
            500,
            ErrorMessage = "The headline cannot exceed 500 characters.")]
        public string? Headline { get; set; }

        [BindProperty]
        [Required(
            ErrorMessage = "Please enter a news claim.")]
        [StringLength(
            5000,
            MinimumLength = 3,
            ErrorMessage =
                "The claim must contain between 3 and 5000 characters.")]
        public string Claim { get; set; } = "";

        [BindProperty]
        [Url(
            ErrorMessage = "Please enter a valid URL.")]
        [StringLength(
            2000,
            ErrorMessage =
                "The source URL cannot exceed 2000 characters.")]
        public string? SourceUrl { get; set; }

        // ============================================================
        // ANALYSIS RESULT
        // ============================================================

        public ClaimAnalysisResult? Result { get; set; }

        public List<NewsArticle> RelatedNews { get; set; } = new();

        public string SearchQuery { get; set; } = "";

        public ClaimComparisonResult? Comparison { get; set; }

        public SourceAnalysisResult? SourceAnalysis { get; set; }

        public MLClassificationResult? MLResult { get; set; }

        // ============================================================
        // GET
        // ============================================================

        public void OnGet()
        {
        }

        // ============================================================
        // POST
        // ============================================================

        public async Task OnPostAsync()
        {
            // --------------------------------------------------------
            // STEP 1: SERVER-SIDE VALIDATION
            // --------------------------------------------------------

            if (!ModelState.IsValid)
            {
                return;
            }

            // --------------------------------------------------------
            // STEP 2: CLAIM VALIDATION
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(Claim))
            {
                ModelState.AddModelError(
                    nameof(Claim),
                    "Please enter a claim to analyze.");

                return;
            }

            Claim = Claim.Trim();

            if (Claim.Length < 3)
            {
                ModelState.AddModelError(
                    nameof(Claim),
                    "The claim must contain at least 3 characters.");

                return;
            }

            // --------------------------------------------------------
            // STEP 3: CLEAN HEADLINE
            // --------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(Headline))
            {
                Headline = Headline.Trim();
            }

            // --------------------------------------------------------
            // STEP 4: SECURE SOURCE URL VALIDATION
            // --------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(SourceUrl))
            {
                SourceUrl = SourceUrl.Trim();

                if (!Uri.TryCreate(
                        SourceUrl,
                        UriKind.Absolute,
                        out Uri? sourceUri))
                {
                    ModelState.AddModelError(
                        nameof(SourceUrl),
                        "Please enter a valid source URL.");

                    return;
                }

                // Only HTTP and HTTPS URLs are allowed.
                if (sourceUri.Scheme != Uri.UriSchemeHttp &&
                    sourceUri.Scheme != Uri.UriSchemeHttps)
                {
                    ModelState.AddModelError(
                        nameof(SourceUrl),
                        "Only HTTP and HTTPS source URLs are allowed.");

                    return;
                }

                // A web URL must contain a valid host.
                if (string.IsNullOrWhiteSpace(sourceUri.Host))
                {
                    ModelState.AddModelError(
                        nameof(SourceUrl),
                        "The source URL must contain a valid website address.");

                    return;
                }
            }

            // ========================================================
            // STEP 5: PRELIMINARY CLAIM DETECTION
            // ========================================================

            Result =
                _detectionService.Detect(Claim);

            // ========================================================
            // STEP 6: MACHINE LEARNING CLASSIFICATION
            // ========================================================

            MLResult =
                await _mlClassificationService
                    .ClassifyAsync(Claim);

            if (MLResult != null && Result != null)
            {
                Result.MLLabel =
                    MLResult.Label;

                Result.MLConfidence =
                    MLResult.Confidence;

                Result.MLModel =
                    MLResult.ModelName;
            }

            // ========================================================
            // STEP 7: EVIDENCE RETRIEVAL
            // ========================================================

            EvidenceRecord evidence =
                await _evidenceProvider
                    .GetEvidenceAsync(Claim);

            // ========================================================
            // STEP 8: BUILD NEWS SEARCH QUERY
            // ========================================================

            SearchQuery =
                _newsQueryService
                    .BuildSearchQuery(Claim);

            // ========================================================
            // STEP 9: RETRIEVE RELATED LIVE NEWS
            // ========================================================

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                RelatedNews =
                    await _newsService
                        .SearchNewsAsync(SearchQuery);
            }

            // ========================================================
            // STEP 10: CLAIM-TO-NEWS COMPARISON
            // ========================================================

            Comparison =
                _claimComparisonService
                    .Compare(
                        Claim,
                        RelatedNews);

            // ========================================================
            // STEP 11: SOURCE TRANSPARENCY ANALYSIS
            // ========================================================

            SourceAnalysis =
                _sourceAnalysisService
                    .Analyze(RelatedNews);

            // ========================================================
            // STEP 12: APPLY EVIDENCE TO RESULT
            // ========================================================

            if (Result != null)
            {
                Result.Evidence =
                    evidence.Evidence;

                Result.Source =
                    evidence.Source;

                Result.EvidenceStatus =
                    evidence.EvidenceType;
            }

            // ========================================================
            // STEP 13: ANALYZE RETRIEVED NEWS EVIDENCE
            // ========================================================

            if (Result != null)
            {
                Result =
                    _evidenceAnalysisService
                        .AnalyzeEvidence(
                            Result,
                            RelatedNews);
            }

            // ========================================================
            // STEP 14: FINAL EVIDENCE-BASED VERIFICATION
            // ========================================================

            if (Result != null)
            {
                Result =
                    _verificationService
                        .Verify(
                            Claim,
                            Result,
                            evidence);
            }

            // ========================================================
            // STEP 15: SMART CONFIDENCE SCORING
            // ========================================================

            if (Result != null)
            {
                Result.Confidence =
                    _resultScoringService
                        .CalculateConfidence(
                            Result,
                            MLResult,
                            evidence,
                            Comparison,
                            SourceAnalysis);
            }

            // ========================================================
            // STEP 16: ADD CLAIM COMPARISON CONTEXT
            // ========================================================

            if (Result != null &&
                Comparison != null &&
                RelatedNews.Count > 0 &&
                !string.IsNullOrWhiteSpace(
                    Comparison.Summary))
            {
                Result.Explanation =
                    $"{Result.Explanation} " +
                    Comparison.Summary;
            }

            // ========================================================
            // STEP 17: SAVE VERIFICATION RESULT
            // ========================================================

            if (Result != null)
            {
                var detectionResult =
                    new DetectionResult
                    {
                        Headline =
                            Headline,

                        Claim =
                            Result.Claim,

                        SourceUrl =
                            SourceUrl,

                        Result =
                            Result.Verdict,

                        Reason =
                            Result.Explanation,

                        Confidence =
                            Result.Confidence,

                        Evidence =
                            Result.Evidence,

                        Source =
                            Result.Source,

                        CheckedDate =
                            DateTime.UtcNow
                    };

                _dbContext
                    .DetectionResults
                    .Add(detectionResult);

                await _dbContext
                    .SaveChangesAsync();
            }
        }
    }
}