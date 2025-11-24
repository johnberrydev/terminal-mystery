using Spectre.Console;
using TerminalMystery.Models;

namespace TerminalMystery.Services;

public class TerminalUI
{
    private const string GameTitle = "TERMINAL MYSTERY";
    private const string Version = "v1.0";
    
    public void ClearScreen()
    {
        AnsiConsole.Clear();
    }
    
    public void ShowMainMenu()
    {
        ClearScreen();
        ShowTitleScreen();
    }
    
    public void ShowTitleScreen()
    {
        var titleArt = """
        
        ╔══════════════════════════════════════════════════════════════════════════════╗
        ║                                                                              ║
        ║   ▄▄▄█████▓▓█████  ██▀███   ███▄ ▄███▓ ██▓ ███▄    █  ▄▄▄       ██▓         ║
        ║   ▓  ██▒ ▓▒▓█   ▀ ▓██ ▒ ██▒▓██▒▀█▀ ██▒▓██▒ ██ ▀█   █ ▒████▄    ▓██▒         ║
        ║   ▒ ▓██░ ▒░▒███   ▓██ ░▄█ ▒▓██    ▓██░▒██▒▓██  ▀█ ██▒▒██  ▀█▄  ▒██░         ║
        ║   ░ ▓██▓ ░ ▒▓█  ▄ ▒██▀▀█▄  ▒██    ▒██ ░██░▓██▒  ▐▌██▒░██▄▄▄▄██ ▒██░         ║
        ║     ▒██▒ ░ ░▒████▒░██▓ ▒██▒▒██▒   ░██▒░██░▒██░   ▓██░ ▓█   ▓██▒░██████▒     ║
        ║     ▒ ░░   ░░ ▒░ ░░ ▒▓ ░▒▓░░ ▒░   ░  ░░▓  ░ ▒░   ▒ ▒  ▒▒   ▓▒█░░ ▒░▓  ░     ║
        ║       ░     ░ ░  ░  ░▒ ░ ▒░░  ░      ░ ▒ ░░ ░░   ░ ▒░  ▒   ▒▒ ░░ ░ ▒  ░     ║
        ║     ░         ░     ░░   ░ ░      ░    ▒ ░   ░   ░ ░   ░   ▒     ░ ░        ║
        ║               ░  ░   ░            ░    ░           ░       ░  ░    ░  ░     ║
        ║                                                                              ║
        ║            ███▄ ▄███▓▓██   ██▓  ██████ ▄▄▄█████▓▓█████  ██▀███ ▓██   ██▓    ║
        ║           ▓██▒▀█▀ ██▒ ▒██  ██▒▒██    ▒ ▓  ██▒ ▓▒▓█   ▀ ▓██ ▒ ██▒▒██  ██▒    ║
        ║           ▓██    ▓██░  ▒██ ██░░ ▓██▄   ▒ ▓██░ ▒░▒███   ▓██ ░▄█ ▒ ▒██ ██░    ║
        ║           ▒██    ▒██   ░ ▐██▓░  ▒   ██▒░ ▓██▓ ░ ▒▓█  ▄ ▒██▀▀█▄   ░ ▐██▓░    ║
        ║           ▒██▒   ░██▒  ░ ██▒▓░▒██████▒▒  ▒██▒ ░ ░▒████▒░██▓ ▒██▒ ░ ██▒▓░    ║
        ║           ░ ▒░   ░  ░   ██▒▒▒ ▒ ▒▓▒ ▒ ░  ▒ ░░   ░░ ▒░ ░░ ▒▓ ░▒▓░  ██▒▒▒     ║
        ║           ░  ░      ░ ▓██ ░▒░ ░ ░▒  ░ ░    ░     ░ ░  ░  ░▒ ░ ▒░▓██ ░▒░     ║
        ║           ░      ░    ▒ ▒ ░░  ░  ░  ░    ░         ░     ░░   ░ ▒ ▒ ░░      ║
        ║                  ░    ░ ░           ░              ░  ░   ░     ░ ░         ║
        ║                       ░ ░                                       ░ ░         ║
        ║                                                                              ║
        ╠══════════════════════════════════════════════════════════════════════════════╣
        ║                                                                              ║
        ║     "WHAT IS THE PRIMARY GOAL?"                                              ║
        ║                                                                              ║
        ║     You've discovered something you weren't meant to find...                 ║
        ║     A terminal. A connection. A mystery waiting to be unraveled.             ║
        ║                                                                              ║
        ║                        [  Press any key to continue  ]                       ║
        ║                                                                              ║
        ╚══════════════════════════════════════════════════════════════════════════════╝
        
        """;
        
        AnsiConsole.Write(new Markup($"[green]{titleArt.EscapeMarkup()}[/]"));
        Console.ReadKey(true);
    }

    public string ShowMenuOptions(List<GameSession> savedSessions)
    {
        ClearScreen();
        
        var menuHeader = """
        
        ╔══════════════════════════════════════════════════════════════════╗
        ║                     TERMINAL MYSTERY v1.0                        ║
        ║                        MAIN TERMINAL                             ║
        ╠══════════════════════════════════════════════════════════════════╣
        ║                                                                  ║
        """;
        
        AnsiConsole.Write(new Markup($"[green]{menuHeader.EscapeMarkup()}[/]"));
        
        var choices = new List<string> { ">> NEW CONNECTION - Begin a new session" };
        
        if (savedSessions.Count > 0)
        {
            foreach (var session in savedSessions.Take(5))
            {
                var lastPlayed = session.LastPlayedAt.ToString("yyyy-MM-dd HH:mm");
                choices.Add($">> RESUME: {session.Name} (Turn {session.TurnCount}, {lastPlayed})");
            }
        }
        
        choices.Add(">> EXIT - Disconnect");
        
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]║  Select operation:[/]")
                .PageSize(10)
                .HighlightStyle(new Style(Color.Black, Color.Green))
                .AddChoices(choices));
        
        var menuFooter = """
        ║                                                                  ║
        ╚══════════════════════════════════════════════════════════════════╝
        """;
        AnsiConsole.Write(new Markup($"[green]{menuFooter.EscapeMarkup()}[/]"));
        
        return selection;
    }
    
    public void ShowConnecting()
    {
        ClearScreen();
        
        AnsiConsole.Write(new Markup("[green]"));
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("  ESTABLISHING CONNECTION...");
        AnsiConsole.WriteLine();
        
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .Start("[green]Initializing modem...[/]", ctx =>
            {
                Thread.Sleep(800);
                ctx.Status("[green]Dialing remote host...[/]");
                Thread.Sleep(1000);
                ctx.Status("[green]Negotiating protocol...[/]");
                Thread.Sleep(600);
                ctx.Status("[green]Authenticating...[/]");
                Thread.Sleep(500);
                ctx.Status("[green]Connection established![/]");
                Thread.Sleep(400);
            });
        
        AnsiConsole.Write(new Markup("[/]"));
    }
    
    public void ShowGeneratingNarrative()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots2)
            .SpinnerStyle(Style.Parse("green"))
            .Start("[green]Synchronizing with remote system...[/]", ctx =>
            {
                Thread.Sleep(1500);
                ctx.Status("[green]Downloading system manifest...[/]");
                Thread.Sleep(1200);
                ctx.Status("[green]Analyzing system architecture...[/]");
                Thread.Sleep(1000);
            });
    }
    
    public void ShowWelcomeBanner(GameSession session)
    {
        ClearScreen();
        
        var banner = $"""
        
        ╔══════════════════════════════════════════════════════════════════════════════╗
        ║                                                                              ║
        ║   ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░   ║
        ║   ░  CONNECTION ESTABLISHED - {session.SystemHostname,-15}                       ░   ║
        ║   ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░   ║
        ║                                                                              ║
        ║   WARNING: UNAUTHORIZED ACCESS WILL BE MONITORED AND TRACED                  ║
        ║                                                                              ║
        ║   System: {session.Narrative.OrganizationName,-20}                             ║
        ║   Terminal: {session.SystemHostname,-18}                                       ║
        ║   Access Level: GUEST                                                        ║
        ║                                                                              ║
        ║   Type 'help' for available commands                                         ║
        ║   Press Ctrl+G or type '!menu' to access game menu                           ║
        ║                                                                              ║
        ╚══════════════════════════════════════════════════════════════════════════════╝
        
        """;
        
        AnsiConsole.Write(new Markup($"[green]{banner.EscapeMarkup()}[/]"));
    }
    
    public string GetPrompt(GameSession session)
    {
        return $"[green]{session.Username}@{session.SystemHostname}:{session.CurrentDirectory}$[/] ";
    }
    
    public string ReadCommand(GameSession session)
    {
        AnsiConsole.Write(new Markup(GetPrompt(session)));
        var input = Console.ReadLine() ?? string.Empty;
        return input.Trim();
    }
    
    public void WriteResponse(string response)
    {
        // Parse response for special markers
        var displayResponse = response;
        
        // Remove hidden markers from display
        var lines = displayResponse.Split('\n')
            .Where(l => !l.Contains("[CLUE_DISCOVERED:") && 
                       !l.Contains("[TASK_COMPLETED:") &&
                       !l.Contains("[CD:"))
            .ToArray();
        
        displayResponse = string.Join('\n', lines);
        
        AnsiConsole.WriteLine();
        
        // Color code the response for atmosphere
        foreach (var line in displayResponse.Split('\n'))
        {
            if (line.Contains("ERROR") || line.Contains("DENIED") || line.Contains("WARNING"))
            {
                AnsiConsole.Write(new Markup($"[red]{line.EscapeMarkup()}[/]\n"));
            }
            else if (line.Contains("ACCESS GRANTED") || line.Contains("SUCCESS") || line.Contains("DECRYPTED"))
            {
                AnsiConsole.Write(new Markup($"[lime]{line.EscapeMarkup()}[/]\n"));
            }
            else if (line.Contains("CLASSIFIED") || line.Contains("TOP SECRET") || line.Contains("EYES ONLY"))
            {
                AnsiConsole.Write(new Markup($"[yellow]{line.EscapeMarkup()}[/]\n"));
            }
            else if (line.StartsWith(">>>") || line.StartsWith("---") || line.StartsWith("==="))
            {
                AnsiConsole.Write(new Markup($"[cyan]{line.EscapeMarkup()}[/]\n"));
            }
            else
            {
                AnsiConsole.Write(new Markup($"[green]{line.EscapeMarkup()}[/]\n"));
            }
        }
        
        AnsiConsole.WriteLine();
    }
    
    public void ShowGameMenu()
    {
        AnsiConsole.WriteLine();
        var menuBox = """
        ╔════════════════════════════════════╗
        ║          GAME MENU                 ║
        ╠════════════════════════════════════╣
        ║  [S] Save Session                  ║
        ║  [R] Return to Game                ║
        ║  [Q] Save & Quit to Main Menu      ║
        ║  [X] Quit Without Saving           ║
        ╚════════════════════════════════════╝
        """;
        AnsiConsole.Write(new Markup($"[yellow]{menuBox.EscapeMarkup()}[/]"));
    }
    
    public char GetMenuChoice()
    {
        AnsiConsole.Write(new Markup("[yellow]Select option: [/]"));
        var key = Console.ReadKey(true);
        AnsiConsole.WriteLine(key.KeyChar.ToString().ToUpper());
        return char.ToUpper(key.KeyChar);
    }
    
    public void ShowSaveConfirmation()
    {
        AnsiConsole.Write(new Markup("[lime]Session saved successfully.[/]\n"));
        Thread.Sleep(1000);
    }
    
    public void ShowError(string message)
    {
        AnsiConsole.Write(new Markup($"[red]ERROR: {message.EscapeMarkup()}[/]\n"));
    }
    
    public void ShowStatus(string message)
    {
        AnsiConsole.Write(new Markup($"[cyan]{message.EscapeMarkup()}[/]\n"));
    }
    
    public string PromptSessionName()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Enter a name for this session:[/]")
                .DefaultValue($"Session-{DateTime.Now:yyyyMMdd-HHmm}")
                .ValidationErrorMessage("[red]Please enter a valid name[/]"));
    }

    public void ShowPhaseTransition(GamePhase newPhase)
    {
        var phaseMessages = new Dictionary<GamePhase, string>
        {
            { GamePhase.Investigation, ">>> SECURITY LEVEL ELEVATED - NEW AREAS ACCESSIBLE <<<" },
            { GamePhase.Revelation, ">>> CRITICAL DATA DISCOVERED - PROCEED WITH CAUTION <<<" },
            { GamePhase.Dilemma, ">>> DECISION REQUIRED - YOUR CHOICE WILL HAVE CONSEQUENCES <<<" },
            { GamePhase.Conclusion, ">>> ENDGAME PROTOCOL INITIATED <<<" }
        };
        
        if (phaseMessages.TryGetValue(newPhase, out var message))
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Markup($"[yellow bold]{message.EscapeMarkup()}[/]\n"));
            AnsiConsole.WriteLine();
            Thread.Sleep(2000);
        }
    }
    
    public void ShowConfigError()
    {
        ClearScreen();
        var errorBox = """
        
        ╔══════════════════════════════════════════════════════════════════╗
        ║                      CONFIGURATION ERROR                         ║
        ╠══════════════════════════════════════════════════════════════════╣
        ║                                                                  ║
        ║   Unable to load API configuration.                              ║
        ║                                                                  ║
        ║   Please edit 'config.json' and add your Gemini API key:         ║
        ║                                                                  ║
        ║   {                                                              ║
        ║     "geminiApiKey": "YOUR_API_KEY_HERE",                         ║
        ║     "modelName": "gemini-2.0-flash",                             ║
        ║     "savesDirectory": "saves"                                    ║
        ║   }                                                              ║
        ║                                                                  ║
        ║   Get your API key at: https://aistudio.google.com/apikey        ║
        ║                                                                  ║
        ╚══════════════════════════════════════════════════════════════════╝
        
        """;
        AnsiConsole.Write(new Markup($"[red]{errorBox.EscapeMarkup()}[/]"));
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup("[grey]Press any key to exit...[/]"));
        Console.ReadKey(true);
    }
}
