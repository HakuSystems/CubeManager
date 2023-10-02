using System.Windows;
using CubeManager.Helpers;
using CubeManager.Notifications;
using Wpf.Ui.Controls;

namespace CubeManager;

public partial class InitialWindow : UiWindow
{
    private readonly Logger _logger;

    public InitialWindow()
    {
        _logger = new Logger();
        NotificationHandler = new NotificationHandler();
        InitializeComponent();
        _logger.Info("InitialWindow initialized");
    }

    private NotificationHandler NotificationHandler { get; set; }

    private void InitialWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _logger.Info("InitialWindow loaded");
        var mainWindow = new SplashScreen();
        mainWindow.Show();
        _logger.Info("SplashScreen shown");
        Close();
        _logger.Info("InitialWindow closed");
    }
}