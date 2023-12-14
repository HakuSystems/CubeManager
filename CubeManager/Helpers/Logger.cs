using System.IO;
using Serilog;
using System.Windows;
using CubeManager.CustomMessageBox;

namespace CubeManager.Helpers
{
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

        public void Error(string title,string message)
        {
            Log.Logger.Error(message);
            HandleErrorAndCritical(title,message);
        }

        public void Critical(string title, string message)
        {
            Log.Logger.Fatal(message);
            HandleErrorAndCritical(title,message);
        }

        private void HandleErrorAndCritical(string title, string message)
        {
            var customMessageBoxWindow = new CubeMessageBox
            {
                TitleText = {Text = title},
                MessageText = {Text = message}
            };

            customMessageBoxWindow.ShowDialog();
        }
    }
}