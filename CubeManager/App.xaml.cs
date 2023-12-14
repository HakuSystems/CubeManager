using System;
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
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogException(e.Exception);
        e.Handled = true;
    }
    
    private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        LogException((Exception)e.ExceptionObject);
    }

    private void LogException(Exception ex)
    {
        // Use this to log the exception details to the desired location
        // such as a file, database, or cloud logging service.

        // Present a more user-friendly message to the user.
        var customMessageBoxWindow = new CubeMessageBox
        {
            TitleText = {Text = "Error"},
            MessageText = {Text = $"An unhandled exception occurred: {ex.Message}"}
        };

        customMessageBoxWindow.ShowDialog();
    }
}