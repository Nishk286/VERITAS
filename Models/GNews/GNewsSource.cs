using System.Text.Json.Serialization;

namespace VERITAS.Models.GNews
{
    public class GNewsSource
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("url")]
        public string Url { get; set; } = "";
    }
}