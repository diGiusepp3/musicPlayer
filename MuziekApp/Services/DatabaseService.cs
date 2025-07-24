using System.Net.Http.Json;

namespace MuziekApp.Services
{
    public class DatabaseService
    {
        private readonly HttpClient _httpClient;

        public DatabaseService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://music.datadrive.be/api/")
            };
        }

        /// <summary>
        /// Registreert een nieuwe gebruiker via de API.
        /// </summary>
        public async Task<bool> RegisterUserAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("register.php", new { email, password });
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
            return result?.Status == "ok";
        }

        /// <summary>
        /// Logt een gebruiker in via de API.
        /// </summary>
        public async Task<bool> LoginUserAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("login.php", new { email, password });
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
            return result?.Status == "ok";
        }

        /// <summary>
        /// Controleert of de API bereikbaar is en de database in orde is.
        /// </summary>
        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse>("dbcheck.php");
                return response?.Status == "ok";
            }
            catch
            {
                return false;
            }
        }

        private class ApiResponse
        {
            public string Status { get; set; }
        }
    }
}