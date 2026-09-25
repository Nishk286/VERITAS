using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VERITAS.Pages
{
    public class ChallengeModel : PageModel
    {
        private readonly List<ChallengeQuestion> _questions =
            new()
            {
                new ChallengeQuestion
                {
                    Title = "A claim spreads rapidly online",

                    Claim =
                        "A widely shared post says that a new scientific discovery has been confirmed.",

                    Context =
                        "The post contains a dramatic headline but does not identify the original study or provide a link to the research.",

                    Difficulty = "MEDIUM",

                    CorrectSourceChoice =
                        "Original source",

                    CorrectEvidenceChoice =
                        "Verifiable evidence",

                    CorrectVerdictChoice =
                        "Insufficient Evidence",

                    Explanation =
                        "The original source should be identified before accepting the claim. " +
                        "Without the underlying study or reliable evidence, the available information " +
                        "is insufficient to verify the discovery."
                },

                new ChallengeQuestion
                {
                    Title = "An old image appears in a new story",

                    Claim =
                        "An image circulating today is presented as evidence of a recent event.",

                    Context =
                        "The image has no clear date or location, and the original publication has not yet been identified.",

                    Difficulty = "MEDIUM",

                    CorrectSourceChoice =
                        "Original source",

                    CorrectEvidenceChoice =
                        "Verifiable evidence",

                    CorrectVerdictChoice =
                        "Insufficient Evidence",

                    Explanation =
                        "An image should be checked for its original source, date, location, and context. " +
                        "Without enough information to establish those details, a recent-event claim " +
                        "should not be treated as verified."
                },

                new ChallengeQuestion
                {
                    Title = "A claim has supporting documentation",

                    Claim =
                        "A factual claim is supported by an identifiable document from a credible primary source.",

                    Context =
                        "The document can be located independently and its relevant information can be checked against the claim.",

                    Difficulty = "HARD",

                    CorrectSourceChoice =
                        "Original source",

                    CorrectEvidenceChoice =
                        "Verifiable evidence",

                    CorrectVerdictChoice =
                        "Verified",

                    Explanation =
                        "An identifiable primary source provides a basis for checking the claim directly. " +
                        "When the supporting document can be independently located and its relevant " +
                        "information matches the claim, the claim can be treated as verified within the " +
                        "scope of the available evidence."
                }
            };


        // =========================================================
        // CURRENT QUESTION
        // =========================================================

        [BindProperty]
        public int CurrentQuestionNumber { get; set; } = 1;


        // =========================================================
        // USER ANSWERS
        // =========================================================

        [BindProperty]
        [Required(ErrorMessage = "Please select an answer.")]
        public string? SourceChoice { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Please select an answer.")]
        public string? EvidenceChoice { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Please select an answer.")]
        public string? VerdictChoice { get; set; }


        // =========================================================
        // RESULT
        // =========================================================

        public bool Completed { get; private set; }


        public int Score { get; private set; }


        public string Feedback { get; private set; } = "";


        public List<ChallengeReviewItem> ReviewAnswers { get; private set; } =
            new();


        // =========================================================
        // TOTALS
        // =========================================================

        public int TotalQuestions
        {
            get
            {
                return _questions.Count;
            }
        }


        public int TotalScore
        {
            get
            {
                return _questions.Count * 3;
            }
        }


        // =========================================================
        // PROGRESS
        // =========================================================

        public int ProgressPercentage
        {
            get
            {
                if (TotalQuestions == 0)
                {
                    return 0;
                }

                return (int)Math.Round(
                    CurrentQuestionNumber /
                    (double)TotalQuestions *
                    100);
            }
        }


        // =========================================================
        // CURRENT QUESTION
        // =========================================================

        public ChallengeQuestion? CurrentQuestion
        {
            get
            {
                if (CurrentQuestionNumber < 1 ||
                    CurrentQuestionNumber > _questions.Count)
                {
                    return null;
                }

                return _questions[
                    CurrentQuestionNumber - 1];
            }
        }


        // =========================================================
        // GET
        // =========================================================

        public void OnGet()
        {
            CurrentQuestionNumber = 1;

            Completed = false;

            Score = 0;

            Feedback = "";

            ReviewAnswers = new();

            TempData.Remove("ChallengeAnswers");
        }


        // =========================================================
        // POST
        // =========================================================

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                return;
            }


            ChallengeQuestion? question =
                CurrentQuestion;


            if (question == null)
            {
                return;
            }


            // -----------------------------------------------------
            // LOAD PREVIOUS ANSWERS
            // -----------------------------------------------------

            List<ChallengeAnswer> answers =
                LoadAnswers();


            // -----------------------------------------------------
            // CALCULATE CURRENT QUESTION SCORE
            // -----------------------------------------------------

            int questionScore =
                CalculateQuestionScore(question);


            // -----------------------------------------------------
            // SAVE CURRENT ANSWER
            // -----------------------------------------------------

            answers.Add(
                new ChallengeAnswer
                {
                    QuestionNumber =
                        CurrentQuestionNumber,

                    SourceChoice =
                        SourceChoice ?? "",

                    EvidenceChoice =
                        EvidenceChoice ?? "",

                    VerdictChoice =
                        VerdictChoice ?? "",

                    Score =
                        questionScore
                });


            SaveAnswers(answers);


            // -----------------------------------------------------
            // MOVE TO NEXT QUESTION
            // -----------------------------------------------------

            if (CurrentQuestionNumber < TotalQuestions)
            {
                CurrentQuestionNumber++;

                SourceChoice = null;

                EvidenceChoice = null;

                VerdictChoice = null;

                return;
            }


            // -----------------------------------------------------
            // FINAL RESULT
            // -----------------------------------------------------

            Score =
                answers.Sum(answer => answer.Score);


            ReviewAnswers =
                BuildReview(answers);


            Completed = true;


            Feedback =
                BuildFeedback(Score);


            // We no longer need the temporary answer storage
            // after the final result has been calculated.

            TempData.Remove("ChallengeAnswers");
        }


        // =========================================================
        // CALCULATE QUESTION SCORE
        // =========================================================

        private int CalculateQuestionScore(
            ChallengeQuestion question)
        {
            int score = 0;


            if (string.Equals(
                    SourceChoice,
                    question.CorrectSourceChoice,
                    StringComparison.Ordinal))
            {
                score++;
            }


            if (string.Equals(
                    EvidenceChoice,
                    question.CorrectEvidenceChoice,
                    StringComparison.Ordinal))
            {
                score++;
            }


            if (string.Equals(
                    VerdictChoice,
                    question.CorrectVerdictChoice,
                    StringComparison.Ordinal))
            {
                score++;
            }


            return score;
        }


        // =========================================================
        // LOAD ANSWERS
        // =========================================================

        private List<ChallengeAnswer> LoadAnswers()
        {
            string? json =
                TempData.Peek("ChallengeAnswers") as string;


            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<ChallengeAnswer>();
            }


            try
            {
                return
                    JsonSerializer.Deserialize<
                        List<ChallengeAnswer>>(json)
                    ?? new List<ChallengeAnswer>();
            }
            catch
            {
                return new List<ChallengeAnswer>();
            }
        }


        // =========================================================
        // SAVE ANSWERS
        // =========================================================

        private void SaveAnswers(
            List<ChallengeAnswer> answers)
        {
            string json =
                JsonSerializer.Serialize(answers);


            TempData["ChallengeAnswers"] = json;
        }


        // =========================================================
        // BUILD REVIEW
        // =========================================================

        private List<ChallengeReviewItem> BuildReview(
            List<ChallengeAnswer> answers)
        {
            List<ChallengeReviewItem> review =
                new();


            foreach (ChallengeQuestion question
                in _questions)
            {
                ChallengeAnswer? answer =
                    answers.FirstOrDefault(
                        item =>
                            item.QuestionNumber ==
                            _questions.IndexOf(question) + 1);


                if (answer == null)
                {
                    continue;
                }


                bool sourceCorrect =
                    string.Equals(
                        answer.SourceChoice,
                        question.CorrectSourceChoice,
                        StringComparison.Ordinal);


                bool evidenceCorrect =
                    string.Equals(
                        answer.EvidenceChoice,
                        question.CorrectEvidenceChoice,
                        StringComparison.Ordinal);


                bool verdictCorrect =
                    string.Equals(
                        answer.VerdictChoice,
                        question.CorrectVerdictChoice,
                        StringComparison.Ordinal);


                review.Add(
                    new ChallengeReviewItem
                    {
                        QuestionNumber =
                            answer.QuestionNumber,

                        Title =
                            question.Title,

                        Score =
                            answer.Score,

                        MaximumScore = 3,

                        SourceAnswer =
                            answer.SourceChoice,

                        CorrectSourceAnswer =
                            question.CorrectSourceChoice,

                        SourceCorrect =
                            sourceCorrect,

                        EvidenceAnswer =
                            answer.EvidenceChoice,

                        CorrectEvidenceAnswer =
                            question.CorrectEvidenceChoice,

                        EvidenceCorrect =
                            evidenceCorrect,

                        VerdictAnswer =
                            answer.VerdictChoice,

                        CorrectVerdictAnswer =
                            question.CorrectVerdictChoice,

                        VerdictCorrect =
                            verdictCorrect,

                        Explanation =
                            question.Explanation
                    });
            }


            return review;
        }


        // =========================================================
        // FEEDBACK
        // =========================================================

        private string BuildFeedback(
            int score)
        {
            if (score == TotalScore)
            {
                return
                    "Excellent verification work. You consistently " +
                    "identified original sources, looked for verifiable " +
                    "evidence, and selected conclusions supported by " +
                    "the available information.";
            }


            if (score >= 7)
            {
                return
                    "You demonstrated strong verification habits. " +
                    "Review the areas where you lost points and continue " +
                    "checking sources, evidence, context, and the limits " +
                    "of available information.";
            }


            if (score >= 5)
            {
                return
                    "You identified several important verification " +
                    "principles. Continue practicing source identification, " +
                    "evidence evaluation, and careful verdict selection.";
            }


            return
                "Verification requires more than accepting a headline. " +
                "Focus on the original source, verifiable evidence, " +
                "context, and whether the available information is " +
                "sufficient to support a conclusion.";
        }


        // =========================================================
        // ANSWER MODEL
        // =========================================================

        public class ChallengeAnswer
        {
            public int QuestionNumber { get; set; }

            public string SourceChoice { get; set; } = "";

            public string EvidenceChoice { get; set; } = "";

            public string VerdictChoice { get; set; } = "";

            public int Score { get; set; }
        }


        // =========================================================
        // REVIEW MODEL
        // =========================================================

        public class ChallengeReviewItem
        {
            public int QuestionNumber { get; set; }

            public string Title { get; set; } = "";

            public int Score { get; set; }

            public int MaximumScore { get; set; }

            public string SourceAnswer { get; set; } = "";

            public string CorrectSourceAnswer { get; set; } = "";

            public bool SourceCorrect { get; set; }

            public string EvidenceAnswer { get; set; } = "";

            public string CorrectEvidenceAnswer { get; set; } = "";

            public bool EvidenceCorrect { get; set; }

            public string VerdictAnswer { get; set; } = "";

            public string CorrectVerdictAnswer { get; set; } = "";

            public bool VerdictCorrect { get; set; }

            public string Explanation { get; set; } = "";
        }


        // =========================================================
        // QUESTION MODEL
        // =========================================================

        public class ChallengeQuestion
        {
            public string Title { get; set; } = "";

            public string Claim { get; set; } = "";

            public string Context { get; set; } = "";

            public string Difficulty { get; set; } = "";

            public string CorrectSourceChoice { get; set; } = "";

            public string CorrectEvidenceChoice { get; set; } = "";

            public string CorrectVerdictChoice { get; set; } = "";

            public string Explanation { get; set; } = "";
        }
    }
}