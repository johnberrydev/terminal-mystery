using System.Text.Json;
using TerminalMystery.Models;

namespace TerminalMystery.Services;

public class SaveService
{
    private readonly ConfigService _configService;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };
    
    public SaveService(ConfigService configService)
    {
        _configService = configService;
    }
    
    public async Task SaveSessionAsync(GameSession session)
    {
        session.LastPlayedAt = DateTime.Now;
        var savesDir = _configService.GetSavesDirectory();
        var filePath = Path.Combine(savesDir, $"{session.Id}.json");
        var json = JsonSerializer.Serialize(session, _jsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }
    
    public async Task<GameSession?> LoadSessionAsync(string sessionId)
    {
        var savesDir = _configService.GetSavesDirectory();
        var filePath = Path.Combine(savesDir, $"{sessionId}.json");
        
        if (!File.Exists(filePath))
            return null;
        
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<GameSession>(json);
    }
    
    public async Task<List<GameSession>> GetAllSessionsAsync()
    {
        var sessions = new List<GameSession>();
        var savesDir = _configService.GetSavesDirectory();
        
        if (!Directory.Exists(savesDir))
            return sessions;
        
        foreach (var file in Directory.GetFiles(savesDir, "*.json"))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var session = JsonSerializer.Deserialize<GameSession>(json);
                if (session != null)
                    sessions.Add(session);
            }
            catch
            {
                // Skip corrupted save files
            }
        }
        
        return sessions.OrderByDescending(s => s.LastPlayedAt).ToList();
    }
    
    public async Task DeleteSessionAsync(string sessionId)
    {
        var savesDir = _configService.GetSavesDirectory();
        var filePath = Path.Combine(savesDir, $"{sessionId}.json");
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        await Task.CompletedTask;
    }
}
