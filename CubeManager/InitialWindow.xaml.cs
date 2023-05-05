using System.Windows;

namespace CubeManager;

public partial class InitialWindow : Window
{
    public InitialWindow()
    {
        InitializeComponent();
    }

    private void InitialWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        var mainWindow = new CubeMangerWindow();
        mainWindow.Show();
        Close();
    }
}