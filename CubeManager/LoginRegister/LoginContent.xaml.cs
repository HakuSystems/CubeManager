using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CubeManager.Helpers;
using CubeManager.Notifications;
using Wpf.Ui.Controls;

namespace CubeManager.LoginRegister;

public partial class LoginContent : UiPage
{
    private SoundManager SoundManager { get; } = new();
    private ConfigManager ConfigManager { get; } = ConfigManager.Instance;
    public LoginContent()
    {
        InitializeComponent();
    }

    private void LoginBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        var loginWindow = (LoginWindow)Window.GetWindow(this);
        loginWindow.MainContentFrame.Source =
            new Uri("pack://application:,,,/CubeManager;component/Settings/SettingsSplashScreen.xaml",
                UriKind.RelativeOrAbsolute);
    }

    private void RegisterBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        var loginWindow = (LoginWindow)Window.GetWindow(this);
        loginWindow.PageContent.Source = new Uri("RegisterContent.xaml", UriKind.RelativeOrAbsolute);
    }

    private void LoginBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void RegisterBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }
}