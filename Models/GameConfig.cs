using System.Text.Json.Serialization;

namespace TerminalMystery.Models;

public class GameConfig
{
    [JsonPropertyName("geminiApiKey")]
    public string GeminiApiKey { get; set; } = string.Empty;
    
    [JsonPropertyName("narrativeModel")]
    public string NarrativeModel { get; set; } = "gemini-2.5-pro";
    
    [JsonPropertyName("commandModel")]
    public string CommandModel { get; set; } = "gemini-2.5-flash";
    
    [JsonPropertyName("savesDirectory")]
    public string SavesDirectory { get; set; } = "saves";
}
