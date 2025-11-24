using System.Text.Json.Serialization;

namespace TerminalMystery.Models;

public class GameConfig
{
    [JsonPropertyName("geminiApiKey")]
    public string GeminiApiKey { get; set; } = string.Empty;
    
    [JsonPropertyName("modelName")]
    public string ModelName { get; set; } = "gemini-2.0-flash";
    
    [JsonPropertyName("savesDirectory")]
    public string SavesDirectory { get; set; } = "saves";
}
