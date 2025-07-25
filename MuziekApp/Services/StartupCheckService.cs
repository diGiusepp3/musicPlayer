using MuziekApp.Services;
using Microsoft.Extensions.Logging;

namespace MuziekApp;

public class StartupCheckService
{
    private readonly DatabaseService _databaseService;
    private readonly ILogger<StartupCheckService> _logger;

    public StartupCheckService(ILogger<StartupCheckService> logger)
    {
        _databaseService = new DatabaseService();
        _logger = logger;
    }

    public async Task<bool> RunCheckAsync()
    {
        try
        {
            bool dbOk = await _databaseService.CheckConnectionAsync();

            if (dbOk)
            {
                _logger.LogInformation("Startup check success (API + DB OK)");
                return true;
            }
            else
            {
                _logger.LogWarning("Startup check failed (API of DB error)");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Startup check API fout: {Message}", ex.Message);
            return false;
        }
    }
}