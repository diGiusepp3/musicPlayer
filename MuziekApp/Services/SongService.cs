using System.Net.Http.Json;

namespace MuziekApp.Services
{
    public class SongDto   // <-- naam aangepast
    {
        public int song_id { get; set; }
        public string title { get; set; }
        public string artists { get; set; }
        public string file_url { get; set; }
        public string cover_url { get; set; }
    }

    public class SongResponse
    {
        public bool success { get; set; }
        public List<SongDto> songs { get; set; }
    }

    public class SongService
    {
        private readonly HttpClient _http = new();

        public async Task<List<SongDto>> GetAllSongsAsync()
        {
            var result = await _http.GetFromJsonAsync<SongResponse>(
                "https://music.datadrive.be/api/songs/get_all.php");
            return result?.songs ?? new List<SongDto>();
        }
    }
}