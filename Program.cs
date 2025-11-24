using TerminalMystery;

// Set console encoding to support Unicode characters
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

// Handle Ctrl+C gracefully
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    Console.WriteLine("\n\nConnection terminated by user.");
    Environment.Exit(0);
};

// Run the game
var game = new GameEngine();
await game.RunAsync();
