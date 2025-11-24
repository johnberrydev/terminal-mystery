# Terminal Mystery

A retro-styled text adventure game where you explore mysterious computer terminals from the 1980s/90s era. Each playthrough features a unique AI-generated narrative involving government secrets, conspiracies, and ethical dilemmas.

## Features

- **Procedurally Generated Narratives**: Each new game creates a unique mystery using Google Gemini AI
- **Retro Terminal Aesthetics**: Authentic 80s/90s mainframe feel with ASCII art and color effects
- **Multiple Save Slots**: Save and resume your sessions at any time
- **Bash/DOS Command Support**: Use familiar terminal commands to explore
- **Five-Phase Story Arc**: Discovery → Investigation → Revelation → Dilemma → Conclusion
- **Ethical Choices**: Face moral dilemmas that affect the story's outcome

## Plot Themes

The game randomly selects from various mystery themes including:
- Government military defense systems (WarGames-style)
- Secret UFO/UAP research programs
- Conspiracy networks and shadow organizations
- International crime syndicates
- Corporate espionage operations
- And many more...

## Installation

### Prerequisites
- .NET 9.0 SDK or later
- A Google Gemini API key

### Setup

1. Clone the repository
2. Edit `config.json` and add your Gemini API key:
   ```json
   {
     "geminiApiKey": "YOUR_ACTUAL_API_KEY",
     "modelName": "gemini-2.0-flash",
     "savesDirectory": "saves"
   }
   ```
3. Get your API key from: https://aistudio.google.com/apikey

### Running the Game

```bash
dotnet run
```

Or build and run the executable:
```bash
dotnet build -c Release
./bin/Release/net9.0/TerminalMystery
```

## How to Play

### Starting a Session
1. Launch the game to see the title screen
2. Select "NEW CONNECTION" to start a new mystery
3. Enter a name for your session

### Commands
The terminal accepts both Unix/Bash and DOS-style commands. Examples:
- `ls` or `dir` - List files
- `cd <directory>` - Change directory
- `cat <file>` or `type <file>` - Read file contents
- `help` - Show available commands
- `pwd` - Show current directory

### Game Menu
- Type `!menu` or `!m` at any prompt to open the game menu
- From here you can:
  - **[S]** Save your session
  - **[R]** Return to the game
  - **[Q]** Save and quit to main menu
  - **[X]** Quit without saving

### Progression
The game progresses through five phases based on turn count:
1. **Discovery** (Turns 1-15): Initial exploration
2. **Investigation** (Turns 16-30): Deeper discoveries
3. **Revelation** (Turns 31-40): Major secrets revealed
4. **Dilemma** (Turns 41-48): Ethical choice presented
5. **Conclusion** (Turns 49+): Dramatic finale

## Technical Details

### Architecture
- **GameEngine.cs**: Main game loop and state management
- **LlmService.cs**: Google Gemini API integration
- **TerminalUI.cs**: Spectre.Console-based terminal interface
- **SaveService.cs**: JSON-based session persistence

### Dependencies
- Mscc.GenerativeAI - Google Gemini SDK
- Spectre.Console - Rich terminal UI
- System.Text.Json - JSON serialization

## License

MIT License

## Acknowledgments

Inspired by classic text adventures and 80s hacker movies like WarGames.
