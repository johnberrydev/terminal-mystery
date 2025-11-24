using Serilog;
using TerminalMystery;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("game.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("Terminal Mystery starting...");

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
try
{
    var game = new GameEngine();
    await game.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Terminal Mystery shutting down");
    await Log.CloseAndFlushAsync();
}
