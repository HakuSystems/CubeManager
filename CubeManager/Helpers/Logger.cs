using System.IO;
using System.Runtime.InteropServices;

namespace CubeManager.Helpers;

public class Logger
{
    private static readonly string LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CubeManager", "Logs");

    public Logger()
    {
        Directory.CreateDirectory(LogDirectory);
        //AllocConsole();
        DeleteOldLogs();
    }


    [DllImport("Kernel32")]
    private static extern void AllocConsole();

    private static ConsoleColor GetLogColor(LogSeverity severity) => severity switch
    {
        LogSeverity.Error or LogSeverity.ErrSilent => ConsoleColor.Red,
        LogSeverity.Critical => ConsoleColor.DarkRed,
        LogSeverity.Warning => ConsoleColor.Yellow,
        LogSeverity.PrioInfo => ConsoleColor.Cyan,
        LogSeverity.Info => ConsoleColor.White,
        LogSeverity.Debug => ConsoleColor.DarkGray,
        _ => ConsoleColor.White
    };

    private static void Log(LogSeverity severity, string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = GetLogColor(severity);

        var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{severity}] {message}";
        Console.WriteLine(logMessage);

        WriteToFile(logMessage);

        Console.ForegroundColor = originalColor;
    }

    private static void WriteToFile(string message)
    {
        var logFile = Path.Combine(LogDirectory, $"log-{DateTime.Today:yyyy-MM-dd}.log");

        using var writer = new StreamWriter(logFile, true);
        writer.WriteLine(message);
    }

    private static void DeleteOldLogs()
    {
        var files = Directory.GetFiles(LogDirectory, "log*.log")
            .OrderByDescending(File.GetCreationTime).Skip(2).ToList();

        files.ForEach(File.Delete);
    }

    public void Info(string message) => Log(LogSeverity.Info, message);
    public void Warn(string message) => Log(LogSeverity.Warning, message);
    public void Error(string message) => Log(LogSeverity.Error, message);
    public void Debug(string message) => Log(LogSeverity.Debug, message);
    public void PrioInfo(string message) => Log(LogSeverity.PrioInfo, message);
    public void Critical(string message) => Log(LogSeverity.Critical, message);
    public void ErrSilent(string message) => Log(LogSeverity.ErrSilent, message);
    

    private enum LogSeverity
    {
        Error,
        ErrSilent,
        Critical,
        Warning,
        PrioInfo,
        Info,
        Debug
    }
}