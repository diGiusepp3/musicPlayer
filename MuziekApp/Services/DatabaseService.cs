using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        /// Checkt verbinding met de API (dbcheck.php).
        /// </summary>
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

        /// <summary>
        /// Registreert een nieuwe gebruiker via de API.
        /// </summary>
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

        /// <summary>
        /// Logt een gebruiker in en geeft de User info terug.
        /// </summary>
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
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsPremium { get; set; }
    }
}
