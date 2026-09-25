using System.Text.Json.Serialization;

namespace VERITAS.Models.GNews
{
    public class GNewsResponse
    {
        [JsonPropertyName("totalArticles")]
        public int TotalArticles { get; set; }

        [JsonPropertyName("articles")]
        public List<GNewsArticle> Articles { get; set; } = new();
    }
}