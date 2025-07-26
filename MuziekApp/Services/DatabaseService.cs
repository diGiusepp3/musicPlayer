using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MuziekApp.Models;

namespace MuziekApp.Services
{
    public class DatabaseService
    {
        private readonly HttpClient _httpClient;

        public DatabaseService()
        {
            // Basis URL naar jouw API
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://music.datadrive.be/api/")
            };
        }

        private class ApiResponse
        {
            public string Status { get; set; }
        }

        private class LoginResponse
        {
            public string Status { get; set; }
            [JsonPropertyName("user")]
            public User User { get; set; }
        }

        // === CONNECTIE TEST ===
        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse>("dbcheck.php");
                return response?.Status == "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckConnection error: " + ex.Message);
                return false;
            }
        }

        // === REGISTER ===
        public async Task<bool> RegisterUserAsync(string email, string password)
        {
            try
            {
                var jsonContent = JsonContent.Create(new { email, password });
                var response = await _httpClient.PostAsync("users/register.php", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Register HTTP error: " + response.StatusCode);
                    return false;
                }

                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine("REGISTER RESPONSE RAW: " + raw);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ApiResponse>(raw, options);

                return result?.Status == "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Register exception: " + ex.Message);
                return false;
            }
        }

        // === LOGIN ===
        public async Task<User?> LoginAndGetUserAsync(string email, string password)
        {
            try
            {
                var jsonContent = JsonContent.Create(new { email, password });
                var response = await _httpClient.PostAsync("users/login.php", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Login HTTP error: " + response.StatusCode);
                    return null;
                }

                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine("LOGIN RESPONSE RAW: " + raw);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<LoginResponse>(raw, options);

                return result?.Status == "ok" ? result.User : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login exception: " + ex.Message);
                return null;
            }
        }

        // === SONG TOEVOEGEN ===
        public async Task<bool> AddSongAsync(int albumId, string title, int duration, int trackNumber, string audioUrl, string filePath)
        {
            try
            {
                var jsonContent = JsonContent.Create(new
                {
                    album_id = albumId,
                    title,
                    duration,
                    track_number = trackNumber,
                    audio_url = audioUrl,
                    file_path = filePath
                });

                var response = await _httpClient.PostAsync("songs/add_song.php", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("AddSong HTTP error: " + response.StatusCode);
                    return false;
                }

                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine("ADD SONG RESPONSE RAW: " + raw);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ApiResponse>(raw, options);

                return result?.Status == "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddSong exception: " + ex.Message);
                return false;
            }
        }

        // === YOUTUBE DOWNLOAD (single track) ===
        public async Task<bool> DownloadFromYouTubeAsync(string youtubeUrl)
        {
            try
            {
                var jsonContent = JsonContent.Create(new { url = youtubeUrl, type = "audio" });
                var response = await _httpClient.PostAsync("functions/download.php?single=1", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("YT Download HTTP error: " + response.StatusCode);
                    return false;
                }

                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine("YT DOWNLOAD RAW: " + raw);

                // We verwachten een JSON met een 'id'
                return raw.Contains("id");
            }
            catch (Exception ex)
            {
                Console.WriteLine("YT Download exception: " + ex.Message);
                return false;
            }
        }

        // === ALLE SONGS OPHALEN ===
        public async Task<List<Song>> GetAllSongsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Song>>("songs/get_all.php");
                return response ?? new List<Song>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllSongs exception: " + ex.Message);
                return new List<Song>();
            }
        }

        public async Task<List<Artist>> GetAllArtistsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Artist>>("artists/get_all.php");
                return response ?? new List<Artist>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllArtists exception: " + ex.Message);
                return new List<Artist>();
            }
        }

        public async Task<List<Album>> GetAllAlbumsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Album>>("albums/get_all.php");
                return response ?? new List<Album>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllAlbums exception: " + ex.Message);
                return new List<Album>();
            }
        }

        // === YOUTUBE SEARCH (via PHP API) ===
        public async Task<List<SearchResultItem>> SearchYouTubeAsync(string query)
        {
            try
            {
                var url = $"functions/search.php?q={Uri.EscapeDataString(query)}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<SearchResultItem>();

                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine("SEARCH RESPONSE RAW: " + raw);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<SearchResultItem>>(raw, options) ?? new List<SearchResultItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Search exception: " + ex.Message);
                return new List<SearchResultItem>();
            }
        }
    }
}
