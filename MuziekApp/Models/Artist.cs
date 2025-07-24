using System.Text.Json.Serialization;

namespace MuziekApp.Models
{
    public class Artist
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }
}