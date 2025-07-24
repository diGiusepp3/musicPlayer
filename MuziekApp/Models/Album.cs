using System.Text.Json.Serialization;

namespace MuziekApp.Models
{
    public class Album
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("cover_url")]
        public string CoverUrl { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }
}