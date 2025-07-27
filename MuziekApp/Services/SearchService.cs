using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MuziekApp.Services
{
    public class SearchService
    {
        private readonly HttpClient _httpClient;

        public SearchService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<SearchResult>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchResult>();

            var url = $"https://music.datadrive.be/api/functions/search.php?q={Uri.EscapeDataString(query)}";
            System.Diagnostics.Debug.WriteLine($"[SEARCH] API Call: {url}");

            try
            {
                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[SEARCH] API Response: {json}");

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("[SEARCH] API call failed!");
                    return new List<SearchResult>();
                }

                var data = System.Text.Json.JsonSerializer.Deserialize<List<SearchResult>>(json);
                return data ?? new List<SearchResult>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SEARCH] Exception: {ex.Message}");
                return new List<SearchResult>();
            }
        }
    }

    public class SearchResult
    {
        [JsonPropertyName("videoId")]
        public string VideoId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }
    }
}