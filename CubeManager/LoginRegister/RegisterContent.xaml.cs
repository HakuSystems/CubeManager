using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CubeManager.API;
using CubeManager.Helpers;
using CubeManager.Settings;
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
        var loginWindow = (LoginWindow)Window.GetWindow(this);
        loginWindow.PageContent.Source = new Uri("LoginContent.xaml", UriKind.Relative);
    }

    private async void RegisterBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);

        var registerSuccessful = await APICalls.Register(UsernameBox.Text, PasswordBox.Password, EmailBox.Text);
        if (!registerSuccessful) return;

        var loginSuccessful = await APICalls.Login(UsernameBox.Text, PasswordBox.Password);
        if (!loginSuccessful) return;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var settingsWindow = SettingsWindow.Instance;
            settingsWindow.Show();
            Window.GetWindow(this).Close();
        });
    }

    private void RegisterBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void LoginBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void EmailBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (InputChecker.ValidateEmail(EmailBox.Text))
        {
            RegisterBtn.IsEnabled = true;
            EmailBox.Background = Brushes.Transparent;
        }
        else
        {
            RegisterBtn.IsEnabled = false;
            EmailBox.Background = Brushes.DarkRed;
        }
    }
}