namespace MuziekApp.Services
{
    public static class PlatformPathResolver
    {
        public static string GetBasePath()
        {
#if ANDROID
            // Android externe opslag (zichtbaar)
            var external = Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath;
            return Path.Combine(external ?? FileSystem.Current.AppDataDirectory, "Listen!");
#elif IOS
            // iOS sandbox document directory
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documents, "Listen!");
#else
            // Windows & MacCatalyst
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Listen!");
#endif
        }
    }
}