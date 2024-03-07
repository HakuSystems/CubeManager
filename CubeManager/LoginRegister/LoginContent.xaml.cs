using System.Windows;
using System.Windows.Input;
using CubeManager.API;
using CubeManager.Helpers;
using CubeManager.Settings;
using Wpf.Ui.Controls;

namespace CubeManager.LoginRegister;

public partial class LoginContent : UiPage
{
    //Todo: Handle Login Window not closing after successful login
    private SoundManager SoundManager { get; } = new();

    private ConfigManager ConfigManager { get; } = ConfigManager.Instance;

    public LoginContent()
    {
        InitializeComponent();
    }

    private async void LoginBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SoundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        var loginWindow = Window.GetWindow(LoginWindow.Instance);

        var loginSuccessful = await APICalls.Login(UsernameBox.Text, PasswordBox.Password);
        if (!loginSuccessful) return;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var settingsWindow = SettingsWindow.Instance;
            settingsWindow.Show();
            loginWindow?.Close();
        });
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

    private async void LoginContent_OnLoaded(object sender, RoutedEventArgs e)
    {
        var loginWindow = Window.GetWindow(LoginWindow.Instance);
        if (ConfigManager.Instance.Config.UserData.Username != null)
        {
            LoginWindow.Instance.IsEnabled = false;
            var loginSuccessful = await APICalls.Login(ConfigManager.Instance.Config.UserData.Token);
            if (!loginSuccessful) return;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var settingsWindow = SettingsWindow.Instance;
                settingsWindow.Show();
                loginWindow?.Close();
            });
        }
    }
}