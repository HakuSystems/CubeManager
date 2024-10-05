using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using CubeManager.CustomMessageBox;
using CubeManager.Helpers;
using CubeManager.LoginRegister;
using CubeManager.Notifications;
using Microsoft.AspNetCore.Components.RenderTree;

namespace CubeManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private Logger _logger;
    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        Startup += OnStartup;
    }
    private NotificationHandler NotificationHandler { get; set; }
    private void OnStartup(object sender, StartupEventArgs e)
    {
        _logger = new Logger();
        NotificationHandler = new NotificationHandler();
        _logger.Info("InitialWindow initialized");
        var mainWindow = new LoginWindow();
        mainWindow.Show();
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
        var customMessageBoxWindow = new CubeMessageBox
        {
            TitleText = {Text = "Error"},
            MessageText = {Text = $"An unhandled exception occurred: {ex.Message}"}
        };

        customMessageBoxWindow.ShowDialog();
    }
}