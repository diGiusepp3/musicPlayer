using MySqlConnector;

namespace MuziekApp.Services
{
    public class DatabaseService
    {
        private const string ConnectionString =
            "Server=mgielen.zapto.org;Port=3306;Database=datadrivebe_music;Uid=matthias;Pwd=7824;";

        public async Task<bool> RegisterUserAsync(string email, string password)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE email=@Email", connection);
            checkCmd.Parameters.AddWithValue("@Email", email);
            var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
            if (count > 0) return false;

            var insertCmd = new MySqlCommand(
                "INSERT INTO users (email, password) VALUES (@Email, SHA2(@Password, 256))", connection);
            insertCmd.Parameters.AddWithValue("@Email", email);
            insertCmd.Parameters.AddWithValue("@Password", password);
            await insertCmd.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<bool> LoginUserAsync(string email, string password)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM users WHERE email=@Email AND password=SHA2(@Password, 256)", connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return count > 0;
        }

        /// <summary>
        /// Checkt of er verbinding met de database gemaakt kan worden.
        /// </summary>
        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                await using var connection = new MySqlConnection(ConnectionString);
                await connection.OpenAsync();
                return connection.State == System.Data.ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }
    }
}
