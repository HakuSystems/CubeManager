using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace CubeManager.LoginRegister;

public partial class LoginContent : UiPage
{
    public LoginContent()
    {
        InitializeComponent();
    }

    private void LoginBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var loginWindow = (LoginWindow)Window.GetWindow(this);
        loginWindow.MainContentFrame.Source =
            new Uri("pack://application:,,,/CubeManager;component/Settings/SettingsSplashScreen.xaml",
                UriKind.RelativeOrAbsolute);
    }

    private void RegisterBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var loginWindow = (LoginWindow)Window.GetWindow(this);
        loginWindow.PageContent.Source = new Uri("RegisterContent.xaml", UriKind.RelativeOrAbsolute);
    }
}