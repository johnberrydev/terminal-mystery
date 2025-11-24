using System.Text.Json;
using Mscc.GenerativeAI;
using Serilog;
using TerminalMystery.Models;

namespace TerminalMystery.Services;

public class LlmService
{
    private readonly ConfigService _configService;
    private GenerativeModel? _narrativeModel;
    private GenerativeModel? _commandModel;
    
    private static readonly string[] PlotTypes =
    [
        "Government military defense system (like WarGames WOPR)",
        "Secret UFO/UAP research program",
        "Deep state conspiracy network",
        "International crime syndicate operations",
        "Dark web black market platform",
        "Illuminati shadow organization",
        "Corporate espionage ring",
        "High-frequency trading manipulation scheme",
        "Covert surveillance dragnet program",
        "Bioweapons research facility",
        "AI consciousness experiment gone wrong",
        "Time-travel research project",
        "Parallel dimension communication array",
        "Mind control MKUltra successor program",
        "Nuclear launch failsafe system"
    ];
    
    public LlmService(ConfigService configService)
    {
        _configService = configService;
    }
    
    public void Initialize()
    {
        Log.Information("Initializing LLM service");
        Log.Information("Narrative model: {Model}", _configService.Config.NarrativeModel);
        Log.Information("Command model: {Model}", _configService.Config.CommandModel);
        
        var googleAI = new GoogleAI(_configService.Config.GeminiApiKey);
        _narrativeModel = googleAI.GenerativeModel(model: _configService.Config.NarrativeModel);
        _commandModel = googleAI.GenerativeModel(model: _configService.Config.CommandModel);
        
        Log.Information("LLM service initialized");
    }
    
    public async Task<GameNarrative> GenerateNarrativeAsync()
    {
        var random = new Random();
        var selectedPlot = PlotTypes[random.Next(PlotTypes.Length)];
        
        var prompt = @"You are a creative writer designing a text adventure game narrative. Generate a unique, compelling mystery story for a game where the player has discovered a mysterious computer terminal connected to the internet or via dial-up modem.

Selected plot theme: " + selectedPlot + @"

Create a detailed narrative with the following JSON structure. Be creative and specific. The story should feel authentic to the 1980s-1990s era of computing with appropriate technical details.

Return ONLY valid JSON with this exact structure:
{
    ""plotType"": """ + selectedPlot + @""",
    ""organizationName"": ""A mysterious organization name (e.g., NEXUS-7, ECHELON, PROMETHEUS)"",
    ""systemPurpose"": ""What this computer system is used for (2-3 sentences)"",
    ""backstory"": ""The history of this system and why it exists (3-4 sentences)"",
    ""secretToUncover"": ""The main secret the player will discover (2-3 sentences)"",
    ""ethicalDilemma"": ""A moral choice the player must make near the end (2-3 sentences describing the dilemma and its consequences)"",
    ""keyCharacters"": [""Character 1 with role"", ""Character 2 with role"", ""Character 3 with role""],
    ""cluesAndTasks"": [
        {""type"": ""clue"", ""description"": ""First clue description"", ""triggerCommand"": ""ls or dir command"", ""revealPhase"": 1},
        {""type"": ""task"", ""description"": ""First task description"", ""triggerCommand"": ""cat or type command on a file"", ""revealPhase"": 1},
        {""type"": ""clue"", ""description"": ""Second clue"", ""triggerCommand"": ""read a specific file"", ""revealPhase"": 2},
        {""type"": ""task"", ""description"": ""Second task"", ""triggerCommand"": ""access a subdirectory"", ""revealPhase"": 2},
        {""type"": ""clue"", ""description"": ""Third clue leading to revelation"", ""triggerCommand"": ""decrypt or access restricted area"", ""revealPhase"": 3},
        {""type"": ""task"", ""description"": ""Critical task"", ""triggerCommand"": ""execute a program or script"", ""revealPhase"": 3},
        {""type"": ""clue"", ""description"": ""Final revelation clue"", ""triggerCommand"": ""access top secret file"", ""revealPhase"": 4}
    ],
    ""dramaticReveal"": ""The shocking truth revealed at the climax (2-3 sentences)""
}

Make the clues and tasks feel like authentic terminal commands. Include realistic file names, directory structures, and command patterns.";

        try
        {
            Log.Debug("Calling narrative model API...");
            Log.Debug("Model: {Model}", _configService.Config.NarrativeModel);
            Log.Debug("Prompt length: {Length} chars", prompt.Length);
            
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var task = _narrativeModel!.GenerateContent(prompt);
            
            Log.Debug("Awaiting API response (60s timeout)...");
            var response = await task.WaitAsync(cts.Token);
            
            Log.Debug("API response received, length: {Length}", response.Text?.Length ?? 0);
            Log.Debug("Response preview: {Preview}", response.Text?[..Math.Min(200, response.Text?.Length ?? 0)]);
            
            var jsonText = ExtractJson(response.Text ?? "{}");
            Log.Debug("Extracted JSON length: {Length}", jsonText.Length);
            
            var narrative = JsonSerializer.Deserialize<GameNarrative>(jsonText);
            Log.Information("Narrative deserialized successfully");
            return narrative ?? CreateFallbackNarrative(selectedPlot);
        }
        catch (OperationCanceledException)
        {
            Log.Error("Narrative generation timed out after 60 seconds");
            return CreateFallbackNarrative(selectedPlot);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Narrative generation error: {Type} - {Message}", ex.GetType().Name, ex.Message);
            if (ex.InnerException != null)
            {
                Log.Error("Inner exception: {Type} - {Message}", ex.InnerException.GetType().Name, ex.InnerException.Message);
            }
            return CreateFallbackNarrative(selectedPlot);
        }
    }
    
    public async Task<string> ProcessCommandAsync(GameSession session, string userInput)
    {
        var recentHistory = session.ConversationHistory
            .TakeLast(10)
            .Select(t => $"User: {t.UserInput}\nSystem: {t.SystemResponse}")
            .ToList();
        
        var historyContext = recentHistory.Count > 0 
            ? string.Join("\n\n", recentHistory) 
            : "No previous commands.";

        var phaseGuidance = GetPhaseGuidance(session);
        var availableClues = session.Narrative.CluesAndTasks
            .Where(c => !c.IsDiscovered && c.RevealPhase <= (int)session.CurrentPhase)
            .ToList();
        
        var cluesText = string.Join("\n", availableClues.Select(c => $"- [{c.Type}] {c.Description} (trigger: {c.TriggerCommand})"));
        
        var prompt = @"You are simulating a mysterious 1980s/1990s computer terminal for a text adventure game. The player has discovered this system and is exploring it.

NARRATIVE CONTEXT:
- Organization: " + session.Narrative.OrganizationName + @"
- System Purpose: " + session.Narrative.SystemPurpose + @"
- Backstory: " + session.Narrative.Backstory + @"
- Secret to uncover: " + session.Narrative.SecretToUncover + @"

CURRENT STATE:
- Hostname: " + session.SystemHostname + @"
- Current Directory: " + session.CurrentDirectory + @"
- Username: " + session.Username + @"
- Turn: " + session.TurnCount + @"/50+
- Phase: " + session.CurrentPhase + @" (" + phaseGuidance + @")
- Clues discovered: " + session.DiscoveredClues.Count + @"
- Tasks completed: " + session.CompletedTasks.Count + @"

AVAILABLE CLUES/TASKS TO REVEAL THIS PHASE:
" + cluesText + @"

RECENT CONVERSATION HISTORY:
" + historyContext + @"

PLAYER'S COMMAND: " + userInput + @"

INSTRUCTIONS:
1. Respond as if you ARE the terminal system. Use authentic retro terminal aesthetics.
2. Interpret the player's intent even if the command isn't perfectly formatted (accept both bash and DOS style commands).
3. If the command relates to any available clue/task trigger, reveal that clue naturally through the terminal output.
4. Maintain immersion - errors should feel like real system errors, discoveries should feel earned.
5. Gradually reveal the mystery based on the current phase.
6. If in Dilemma phase (turns 41-48), present the ethical choice: " + session.Narrative.EthicalDilemma + @"
7. If in Conclusion phase (turns 49+), build to the dramatic reveal: " + session.Narrative.DramaticReveal + @"
8. Keep responses concise but atmospheric. Use ASCII art sparingly for impact.
9. Track directory changes - if user runs 'cd', update the path in your response context.

FORMAT YOUR RESPONSE AS:
- Start with the command result/output
- If a clue is discovered, mark it with [CLUE_DISCOVERED: clue_id] at the end (hidden from display)
- If a task is completed, mark it with [TASK_COMPLETED: task_id] at the end (hidden from display)
- If directory changed, include [CD: new_path] at the end

Respond only with the terminal output. Do not break character.";

        try
        {
            var response = await _commandModel!.GenerateContent(prompt);
            return response.Text ?? "SYSTEM ERROR: Response unavailable.";
        }
        catch (Exception ex)
        {
            return $"CONNECTION ERROR: {ex.Message}\nRetrying connection...";
        }
    }

    public async Task<string> GenerateHostnameAsync(string organizationName)
    {
        var prompt = @"Generate a single mysterious computer hostname for an organization called """ + organizationName + @""".
The hostname should feel like it's from the 1980s/1990s era.
Examples: WOPR, NEXUS-PRIME, ECHELON-7, MJ12-NODE3, ZODIAC-MAIN

Return ONLY the hostname, nothing else. No quotes, no explanation.";

        try
        {
            var response = await _narrativeModel!.GenerateContent(prompt);
            return response.Text?.Trim().ToUpper().Replace(" ", "-") ?? "UNKNOWN-SYS";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nHostname generation error: {ex.Message}");
            return "ENIGMA-" + new Random().Next(100, 999);
        }
    }

    private static string GetPhaseGuidance(GameSession session)
    {
        return session.CurrentPhase switch
        {
            GamePhase.Discovery => "Initial exploration - let player discover basic system structure, hint at something hidden",
            GamePhase.Investigation => "Deeper investigation - reveal connections, encrypted files, restricted areas",
            GamePhase.Revelation => "Major reveals - uncover the true purpose, shocking discoveries",
            GamePhase.Dilemma => "Present the ethical choice - player must decide what to do with this knowledge",
            GamePhase.Conclusion => "Dramatic finale - consequences of choice, final revelations",
            _ => "Continue the mystery"
        };
    }

    private static string ExtractJson(string text)
    {
        // Try to extract JSON from markdown code blocks or raw text
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        
        if (start >= 0 && end > start)
        {
            return text[start..(end + 1)];
        }
        
        return text;
    }

    private static GameNarrative CreateFallbackNarrative(string plotType)
    {
        return new GameNarrative
        {
            PlotType = plotType,
            OrganizationName = "PROMETHEUS-X",
            SystemPurpose = "A classified research and monitoring system with unknown objectives.",
            Backstory = "This system was established in 1983 under a classified directive. Its true purpose has been hidden from public records.",
            SecretToUncover = "The system is collecting and analyzing data on a massive scale for purposes unknown to most operators.",
            EthicalDilemma = "You can either expose the system to the public, potentially causing chaos, or destroy the evidence and walk away.",
            KeyCharacters = ["Dr. Sarah Chen - Lead Researcher", "Colonel James Webb - Military Liaison", "ORACLE - The AI assistant"],
            CluesAndTasks =
            [
                new ClueOrTask { Type = "clue", Description = "System manifest file", TriggerCommand = "ls", RevealPhase = 1 },
                new ClueOrTask { Type = "task", Description = "Read the welcome message", TriggerCommand = "cat welcome.txt", RevealPhase = 1 },
                new ClueOrTask { Type = "clue", Description = "Encrypted communications", TriggerCommand = "cd /secure", RevealPhase = 2 },
                new ClueOrTask { Type = "task", Description = "Access personnel files", TriggerCommand = "cat personnel.dat", RevealPhase = 2 },
                new ClueOrTask { Type = "clue", Description = "The truth about the project", TriggerCommand = "decrypt classified.enc", RevealPhase = 3 },
                new ClueOrTask { Type = "task", Description = "Run the analysis program", TriggerCommand = "./analyze", RevealPhase = 3 },
                new ClueOrTask { Type = "clue", Description = "Final revelation", TriggerCommand = "cat EYES_ONLY.txt", RevealPhase = 4 }
            ],
            DramaticReveal = "The system has been monitoring you all along. You are not the first to discover this terminal - you are being tested."
        };
    }
}
