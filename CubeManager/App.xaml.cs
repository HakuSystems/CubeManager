﻿using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

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
        if (MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error) != MessageBoxResult.None)
        {
            Clipboard.SetText(e.Exception.ToString()); 
        }
        
        e.Handled = true;
    }
}