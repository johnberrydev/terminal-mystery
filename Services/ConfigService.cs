using System.Text.Json;
using TerminalMystery.Models;

namespace TerminalMystery.Services;

public class ConfigService
{
    private const string ConfigFileName = "config.json";
    private readonly string _configPath;
    
    public GameConfig Config { get; private set; } = new();
    
    public ConfigService()
    {
        // Try multiple locations for config file
        var possiblePaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, ConfigFileName),
            Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName),
            ConfigFileName
        };
        
        _configPath = possiblePaths.FirstOrDefault(File.Exists) 
                      ?? Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);
    }
    
    public async Task<bool> LoadConfigAsync()
    {
        if (!File.Exists(_configPath))
        {
            await CreateDefaultConfigAsync();
            return false;
        }
        
        try
        {
            var json = await File.ReadAllTextAsync(_configPath);
            Config = JsonSerializer.Deserialize<GameConfig>(json) ?? new GameConfig();
            return !string.IsNullOrWhiteSpace(Config.GeminiApiKey);
        }
        catch
        {
            return false;
        }
    }
    
    private async Task CreateDefaultConfigAsync()
    {
        var defaultConfig = new GameConfig
        {
            GeminiApiKey = "YOUR_API_KEY_HERE",
            ModelName = "gemini-2.0-flash",
            SavesDirectory = "saves"
        };
        
        var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_configPath, json);
    }
    
    public string GetSavesDirectory()
    {
        // Use config file directory as base for saves
        var configDir = Path.GetDirectoryName(_configPath) ?? Directory.GetCurrentDirectory();
        var savesPath = Path.Combine(configDir, Config.SavesDirectory);
        if (!Directory.Exists(savesPath))
        {
            Directory.CreateDirectory(savesPath);
        }
        return savesPath;
    }
}
