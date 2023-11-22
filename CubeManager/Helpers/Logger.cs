using System.IO;
using Serilog;

namespace CubeManager.Helpers;
//todo on Error and Critical handle it properly
public class Logger
{
    public Logger()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
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