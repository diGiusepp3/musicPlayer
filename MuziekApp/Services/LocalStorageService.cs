using System.Text.Json;

namespace MuziekApp.Services
{
    public static class LocalStorageService
    {
        private static readonly string BasePath = PlatformPathResolver.GetBasePath();
        public static string OfflineSongsPath => Path.Combine(BasePath, "offline_songs");
        public static string UserPrefsPath => Path.Combine(BasePath, "user_prefs");
        private static string UserFile => Path.Combine(UserPrefsPath, "user.json");

        public static void Initialize()
        {
            Directory.CreateDirectory(OfflineSongsPath);
            Console.WriteLine($"[INIT] OfflineSongs map: {OfflineSongsPath}");

            Directory.CreateDirectory(UserPrefsPath);
            Console.WriteLine($"[INIT] UserPrefs map: {UserPrefsPath}");
        }

        public static void SaveUser(UserData user)
        {
            var json = JsonSerializer.Serialize(user);
            File.WriteAllText(UserFile, json);
            Console.WriteLine($"[USER] Userdata opgeslagen in: {UserFile}");
        }

        public static UserData? LoadUser()
        {
            if (!File.Exists(UserFile))
            {
                Console.WriteLine("[USER] Geen user.json gevonden");
                return null;
            }

            var json = File.ReadAllText(UserFile);
            Console.WriteLine($"[USER] Userdata geladen vanuit: {UserFile}");
            return JsonSerializer.Deserialize<UserData>(json);
        }

        public static void ClearUser()
        {
            if (File.Exists(UserFile))
            {
                File.Delete(UserFile);
                Console.WriteLine($"[USER] Userdata verwijderd: {UserFile}");
            }
        }
    }

    public class UserData
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Premium { get; set; }
    }
}