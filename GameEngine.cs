using System.Text.RegularExpressions;
using Serilog;
using TerminalMystery.Models;
using TerminalMystery.Services;

namespace TerminalMystery;

public class GameEngine
{
    private readonly ConfigService _configService;
    private readonly SaveService _saveService;
    private readonly LlmService _llmService;
    private readonly TerminalUI _ui;
    
    private GameSession? _currentSession;
    private bool _isRunning = true;
    
    public GameEngine()
    {
        _configService = new ConfigService();
        _saveService = new SaveService(_configService);
        _llmService = new LlmService(_configService);
        _ui = new TerminalUI();
    }
    
    public async Task RunAsync()
    {
        // Load configuration
        var configLoaded = await _configService.LoadConfigAsync();
        if (!configLoaded || string.IsNullOrWhiteSpace(_configService.Config.GeminiApiKey) ||
            _configService.Config.GeminiApiKey == "YOUR_API_KEY_HERE")
        {
            _ui.ShowConfigError();
            return;
        }
        
        // Initialize LLM service
        _llmService.Initialize();
        
        while (_isRunning)
        {
            _ui.ShowMainMenu();
            var savedSessions = await _saveService.GetAllSessionsAsync();
            var selection = _ui.ShowMenuOptions(savedSessions);
            
            await HandleMenuSelectionAsync(selection, savedSessions);
        }
    }
    
    private async Task HandleMenuSelectionAsync(string selection, List<GameSession> savedSessions)
    {
        if (selection.Contains("NEW CONNECTION"))
        {
            await StartNewGameAsync();
        }
        else if (selection.Contains("RESUME:"))
        {
            // Extract session name from selection
            var match = Regex.Match(selection, @"RESUME: (.+?) \(Turn");
            if (match.Success)
            {
                var sessionName = match.Groups[1].Value;
                var session = savedSessions.FirstOrDefault(s => s.Name == sessionName);
                if (session != null)
                {
                    await ResumeGameAsync(session);
                }
            }
        }
        else if (selection.Contains("EXIT"))
        {
            _isRunning = false;
        }
    }
    
    private async Task StartNewGameAsync()
    {
        Log.Information("Starting new game");
        _ui.ShowConnecting();
        _ui.ShowGeneratingNarrative();
        
        Log.Information("Generating narrative...");
        
        // Generate new narrative
        var narrative = await _llmService.GenerateNarrativeAsync();
        
        Log.Information("Narrative generated: {OrgName}", narrative.OrganizationName);
        Log.Information("Generating hostname...");
        
        var hostname = await _llmService.GenerateHostnameAsync(narrative.OrganizationName);
        
        Log.Information("Hostname: {Hostname}", hostname);
        Log.Information("Prompting for session name...");
        
        _currentSession = new GameSession
        {
            Narrative = narrative,
            SystemHostname = hostname,
            Name = _ui.PromptSessionName()
        };
        
        Log.Information("Session created: {SessionName}", _currentSession.Name);
        
        // Save initial session
        await _saveService.SaveSessionAsync(_currentSession);
        
        // Start game loop
        await RunGameLoopAsync();
    }
    
    private async Task ResumeGameAsync(GameSession session)
    {
        _currentSession = session;
        _ui.ShowConnecting();
        await RunGameLoopAsync();
    }
    
    private async Task RunGameLoopAsync()
    {
        if (_currentSession == null) return;
        
        _ui.ShowWelcomeBanner(_currentSession);
        
        var inGameLoop = true;
        
        while (inGameLoop && _isRunning)
        {
            var input = _ui.ReadCommand(_currentSession);
            
            // Check for game menu commands
            if (input.Equals("!menu", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("!m", StringComparison.OrdinalIgnoreCase))
            {
                var shouldContinue = await HandleGameMenuAsync();
                if (!shouldContinue)
                {
                    inGameLoop = false;
                    continue;
                }
                continue;
            }
            
            // Check for empty input
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            
            // Process command through LLM
            await ProcessCommandAsync(input);
        }
    }
    
    private async Task ProcessCommandAsync(string input)
    {
        if (_currentSession == null) return;
        
        _currentSession.TurnCount++;
        
        // Update game phase based on turn count
        UpdateGamePhase();
        
        // Get response from LLM
        var response = await _llmService.ProcessCommandAsync(_currentSession, input);
        
        // Process response markers
        ProcessResponseMarkers(response);
        
        // Store conversation turn
        var turn = new ConversationTurn
        {
            TurnNumber = _currentSession.TurnCount,
            UserInput = input,
            SystemResponse = response,
            Timestamp = DateTime.Now
        };
        _currentSession.ConversationHistory.Add(turn);
        
        // Display response
        _ui.WriteResponse(response);
        
        // Auto-save every 5 turns
        if (_currentSession.TurnCount % 5 == 0)
        {
            await _saveService.SaveSessionAsync(_currentSession);
        }
    }
    
    private void UpdateGamePhase()
    {
        if (_currentSession == null) return;
        
        var oldPhase = _currentSession.CurrentPhase;
        var newPhase = _currentSession.TurnCount switch
        {
            <= 15 => GamePhase.Discovery,
            <= 30 => GamePhase.Investigation,
            <= 40 => GamePhase.Revelation,
            <= 48 => GamePhase.Dilemma,
            _ => GamePhase.Conclusion
        };
        
        if (newPhase != oldPhase)
        {
            _currentSession.CurrentPhase = newPhase;
            _ui.ShowPhaseTransition(newPhase);
        }
    }
    
    private void ProcessResponseMarkers(string response)
    {
        if (_currentSession == null) return;
        
        // Check for clue discoveries
        var clueMatch = Regex.Match(response, @"\[CLUE_DISCOVERED:\s*(\w+)\]");
        if (clueMatch.Success)
        {
            var clueId = clueMatch.Groups[1].Value;
            if (!_currentSession.DiscoveredClues.Contains(clueId))
            {
                _currentSession.DiscoveredClues.Add(clueId);
                var clue = _currentSession.Narrative.CluesAndTasks.FirstOrDefault(c => c.Id == clueId);
                if (clue != null)
                {
                    clue.IsDiscovered = true;
                }
            }
        }
        
        // Check for task completions
        var taskMatch = Regex.Match(response, @"\[TASK_COMPLETED:\s*(\w+)\]");
        if (taskMatch.Success)
        {
            var taskId = taskMatch.Groups[1].Value;
            if (!_currentSession.CompletedTasks.Contains(taskId))
            {
                _currentSession.CompletedTasks.Add(taskId);
                var task = _currentSession.Narrative.CluesAndTasks.FirstOrDefault(t => t.Id == taskId);
                if (task != null)
                {
                    task.IsDiscovered = true;
                }
            }
        }
        
        // Check for directory changes
        var cdMatch = Regex.Match(response, @"\[CD:\s*([^\]]+)\]");
        if (cdMatch.Success)
        {
            _currentSession.CurrentDirectory = cdMatch.Groups[1].Value.Trim();
        }
    }
    
    private async Task<bool> HandleGameMenuAsync()
    {
        _ui.ShowGameMenu();
        var choice = _ui.GetMenuChoice();
        
        switch (choice)
        {
            case 'S':
                if (_currentSession != null)
                {
                    await _saveService.SaveSessionAsync(_currentSession);
                    _ui.ShowSaveConfirmation();
                }
                return true;
            
            case 'R':
                return true;
            
            case 'Q':
                if (_currentSession != null)
                {
                    await _saveService.SaveSessionAsync(_currentSession);
                    _ui.ShowSaveConfirmation();
                }
                return false;
            
            case 'X':
                return false;
            
            default:
                return true;
        }
    }
}
