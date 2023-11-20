using System.IO;
using System.Runtime.InteropServices;
using Serilog;

namespace CubeManager.Helpers;

public class Logger
{
    private static readonly string LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CubeManager", "Logs");

    public Logger()
    {
        AllocConsole();
        DeleteOldLogs();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "CubeManager",
                    "Logs",
                    "log-.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }


    [DllImport("Kernel32")]
    private static extern void AllocConsole();

    private static void DeleteOldLogs()
    {
        var files = Directory.GetFiles(LogDirectory, "log*.log")
            .OrderByDescending(File.GetCreationTime).Skip(2).ToList();

        files.ForEach(File.Delete);
    }

    public void Info(string message)
    {
        Log.Logger.Information(message);
    }

    public void Warn(string message)
    {
        Log.Logger.Warning(message);
    }

    public void Error(string message)
    {
        Log.Logger.Error(message);
    }

    public void Critical(string message)
    {
        Log.Logger.Fatal(message);
    }
}