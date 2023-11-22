using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using CubeManager.CustomMessageBox;

namespace CubeManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var customMessageBoxWindow = new CubeMessageBox();
        customMessageBoxWindow.TitleText.Text = "Error";
        customMessageBoxWindow.MessageText.Text = $"An unhandled exception occurred: {e.Exception.Message}";
        customMessageBoxWindow.ShowDialog();
        
        
        e.Handled = true;
    }
}