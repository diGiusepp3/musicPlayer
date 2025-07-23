using MySqlConnector;

namespace MuziekApp.Services
{
    public class DatabaseService
    {
        private const string ConnectionString =
            "Server=mgielen.zapto.org;Port=3306;Database=datadrivebe_music;Uid=matthias;Pwd=DigiuSeppe2018___;";

        public async Task<bool> RegisterUserAsync(string email, string password)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            // check of user bestaat
            var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE email=@Email", connection);
            checkCmd.Parameters.AddWithValue("@Email", email);
            var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
            if (count > 0) return false;

            // wachtwoord opslaan (voor nu SHA2 hash)
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
    }
}