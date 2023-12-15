using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CubeManager.CustomMessageBox;
using CubeManager.Helpers;
using Wpf.Ui.Controls;

namespace CubeManager.LoginRegister;

public partial class RegisterContent : UiPage
{
    private SoundManager SoundManager { get; } = new();
    private ConfigManager ConfigManager { get; } = ConfigManager.Instance;
    public RegisterContent()
    {
        InitializeComponent();
    }

    private void LoginBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        var loginWindow = (LoginWindow) Window.GetWindow(this);
        loginWindow.PageContent.Source = new Uri("LoginContent.xaml", UriKind.Relative);
    }

    private void RegisterBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        var customMessageBoxWindow = new CubeMessageBox
        {
            TitleText = {Text = "Register"},
            MessageText = {Text = "Registration is currently disabled. Please contact the developer for more information."},
        };

        customMessageBoxWindow.ShowDialog();
    }

    private void RegisterBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void LoginBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }
}