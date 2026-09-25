using System.Net.Http.Json;
using System.Text.Json;
using VERITAS.Models;

namespace VERITAS.Services
{
    public class MLClassificationService
        : IMLClassificationService
    {
        private readonly HttpClient _httpClient;

        public MLClassificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MLClassificationResult> ClassifyAsync(
            string claim)
        {
            // --------------------------------------------------------
            // STEP 1: Validate input
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(claim))
            {
                return CreateUnavailableResult(
                    "No claim was provided to the machine-learning service.");
            }

            claim = claim.Trim();

            // --------------------------------------------------------
            // STEP 2: Call ML API safely
            // --------------------------------------------------------

            try
            {
                var request = new
                {
                    claim = claim
                };

                using HttpResponseMessage response =
                    await _httpClient.PostAsJsonAsync(
                        "classify",
                        request);

                // ----------------------------------------------------
                // STEP 3: Handle unsuccessful HTTP responses
                // ----------------------------------------------------

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUnavailableResult(
                        "The machine-learning service is currently unavailable.");
                }

                // ----------------------------------------------------
                // STEP 4: Read API response
                // ----------------------------------------------------

                string json =
                    await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                {
                    return CreateUnavailableResult(
                        "The machine-learning service returned an empty response.");
                }

                // ----------------------------------------------------
                // STEP 5: Deserialize response
                // ----------------------------------------------------

                MLApiResponse? result =
                    JsonSerializer.Deserialize<MLApiResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result == null)
                {
                    return CreateUnavailableResult(
                        "The machine-learning service returned an invalid response.");
                }

                // ----------------------------------------------------
                // STEP 6: Validate prediction
                // ----------------------------------------------------

                string label =
                    string.IsNullOrWhiteSpace(result.Label)
                        ? "UNCERTAIN"
                        : result.Label.Trim().ToUpperInvariant();

                double confidence =
                    Math.Clamp(
                        result.Confidence,
                        0,
                        100);

                string modelName =
                    string.IsNullOrWhiteSpace(result.ModelName)
                        ? "VERITAS TF-IDF + Logistic Regression"
                        : result.ModelName;

                // ----------------------------------------------------
                // STEP 7: Return prediction
                // ----------------------------------------------------

                return new MLClassificationResult
                {
                    Label = label,
                    Confidence = confidence,
                    Explanation =
                        "Prediction received from the connected " +
                        "VERITAS machine-learning service.",
                    IsAvailable = result.Available,
                    ModelName = modelName
                };
            }
            catch (HttpRequestException)
            {
                return CreateUnavailableResult(
                    "The VERITAS application could not connect to the " +
                    "machine-learning service.");
            }
            catch (TaskCanceledException)
            {
                return CreateUnavailableResult(
                    "The machine-learning service request timed out.");
            }
            catch (JsonException)
            {
                return CreateUnavailableResult(
                    "The machine-learning service returned invalid data.");
            }
            catch
            {
                return CreateUnavailableResult(
                    "The machine-learning service could not complete the prediction.");
            }
        }

        // ============================================================
        // CREATE SAFE UNAVAILABLE RESULT
        // ============================================================

        private MLClassificationResult CreateUnavailableResult(
            string explanation)
        {
            return new MLClassificationResult
            {
                Label = "UNCERTAIN",
                Confidence = 0,
                Explanation = explanation,
                IsAvailable = false,
                ModelName =
                    "VERITAS TF-IDF + Logistic Regression"
            };
        }

        // ============================================================
        // ML API RESPONSE
        // ============================================================

        private class MLApiResponse
        {
            public string Label { get; set; } = "UNCERTAIN";

            public double Confidence { get; set; }

            public string ModelName { get; set; } =
                "VERITAS TF-IDF + Logistic Regression";

            public bool Available { get; set; }
        }
    }
}