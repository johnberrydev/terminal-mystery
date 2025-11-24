using System.Text.Json.Serialization;

namespace TerminalMystery.Models;

public class GameSession
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [JsonPropertyName("lastPlayedAt")]
    public DateTime LastPlayedAt { get; set; } = DateTime.Now;
    
    [JsonPropertyName("turnCount")]
    public int TurnCount { get; set; } = 0;
    
    [JsonPropertyName("narrative")]
    public GameNarrative Narrative { get; set; } = new();
    
    [JsonPropertyName("conversationHistory")]
    public List<ConversationTurn> ConversationHistory { get; set; } = new();
    
    [JsonPropertyName("discoveredClues")]
    public List<string> DiscoveredClues { get; set; } = new();
    
    [JsonPropertyName("completedTasks")]
    public List<string> CompletedTasks { get; set; } = new();
    
    [JsonPropertyName("currentPhase")]
    public GamePhase CurrentPhase { get; set; } = GamePhase.Discovery;
    
    [JsonPropertyName("systemHostname")]
    public string SystemHostname { get; set; } = string.Empty;
    
    [JsonPropertyName("currentDirectory")]
    public string CurrentDirectory { get; set; } = "/";
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = "guest";
}

public class GameNarrative
{
    [JsonPropertyName("plotType")]
    public string PlotType { get; set; } = string.Empty;
    
    [JsonPropertyName("organizationName")]
    public string OrganizationName { get; set; } = string.Empty;
    
    [JsonPropertyName("systemPurpose")]
    public string SystemPurpose { get; set; } = string.Empty;
    
    [JsonPropertyName("backstory")]
    public string Backstory { get; set; } = string.Empty;
    
    [JsonPropertyName("secretToUncover")]
    public string SecretToUncover { get; set; } = string.Empty;
    
    [JsonPropertyName("ethicalDilemma")]
    public string EthicalDilemma { get; set; } = string.Empty;
    
    [JsonPropertyName("keyCharacters")]
    public List<string> KeyCharacters { get; set; } = new();
    
    [JsonPropertyName("cluesAndTasks")]
    public List<ClueOrTask> CluesAndTasks { get; set; } = new();
    
    [JsonPropertyName("dramaticReveal")]
    public string DramaticReveal { get; set; } = string.Empty;
}

public class ClueOrTask
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..6];
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // "clue" or "task"
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("triggerCommand")]
    public string TriggerCommand { get; set; } = string.Empty;
    
    [JsonPropertyName("revealPhase")]
    public int RevealPhase { get; set; } = 1;
    
    [JsonPropertyName("isDiscovered")]
    public bool IsDiscovered { get; set; } = false;
}

public class ConversationTurn
{
    [JsonPropertyName("turnNumber")]
    public int TurnNumber { get; set; }
    
    [JsonPropertyName("userInput")]
    public string UserInput { get; set; } = string.Empty;
    
    [JsonPropertyName("systemResponse")]
    public string SystemResponse { get; set; } = string.Empty;
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public enum GamePhase
{
    Discovery = 1,      // Turns 1-15: Initial exploration
    Investigation = 2,  // Turns 16-30: Deeper investigation
    Revelation = 3,     // Turns 31-40: Major clues revealed
    Dilemma = 4,        // Turns 41-48: Ethical choice presented
    Conclusion = 5      // Turns 49+: Dramatic finale
}
