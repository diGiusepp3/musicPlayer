using System.Text.Json;
using MuziekApp.Models;  // <-- BELANGRIJK: User model van REST gebruiken

namespace MuziekApp.Services
{
    public static class LocalStorageService
    {
        private static readonly string BasePath = PlatformPathResolver.GetBasePath();
        public static string OfflineSongsPath => Path.Combine(BasePath, "offline_songs");
        public static string UserPrefsPath => Path.Combine(BasePath, "user_prefs");
        private static string UserFile => Path.Combine(UserPrefsPath, "user.json");
        private static string VideosFile => Path.Combine(UserPrefsPath, "videos.json");

        public static void Initialize()
        {
            Directory.CreateDirectory(OfflineSongsPath);
            Console.WriteLine($"[INIT] OfflineSongs map: {OfflineSongsPath}");

            Directory.CreateDirectory(UserPrefsPath);
            Console.WriteLine($"[INIT] UserPrefs map: {UserPrefsPath}");
        }

        // === USERDATA (REST API) ===
        public static void SaveUser(User user)
        {
            var json = JsonSerializer.Serialize(user);
            File.WriteAllText(UserFile, json);
            Console.WriteLine($"[USER] Userdata opgeslagen in: {UserFile}");
        }

        public static User? LoadUser()
        {
            if (!File.Exists(UserFile))
            {
                Console.WriteLine("[USER] Geen user.json gevonden");
                return null;
            }

            var json = File.ReadAllText(UserFile);
            Console.WriteLine($"[USER] Userdata geladen vanuit: {UserFile}");
            return JsonSerializer.Deserialize<User>(json);
        }

        public static void ClearUser()
        {
            if (File.Exists(UserFile))
            {
                File.Delete(UserFile);
                Console.WriteLine($"[USER] Userdata verwijderd: {UserFile}");
            }
        }

        // === GEDOWNLOADE VIDEO'S ===
        public static List<DownloadedVideo> LoadDownloadedVideos()
        {
            if (!File.Exists(VideosFile)) return new List<DownloadedVideo>();
            var json = File.ReadAllText(VideosFile);
            return JsonSerializer.Deserialize<List<DownloadedVideo>>(json) ?? new List<DownloadedVideo>();
        }

        public static void AddDownloadedVideo(DownloadedVideo video)
        {
            var list = LoadDownloadedVideos();
            if (!list.Any(v => v.Url == video.Url))
                list.Add(video);

            File.WriteAllText(VideosFile, JsonSerializer.Serialize(list));
            Console.WriteLine($"[VIDEOS] Video toegevoegd: {video.Title}");
        }
    }

    public class DownloadedVideo
    {
        public string Title { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
    }
}
