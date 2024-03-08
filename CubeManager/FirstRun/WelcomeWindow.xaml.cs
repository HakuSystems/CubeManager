using System.Windows;
using System.Windows.Input;
using CubeManager.API;
using CubeManager.CubeManagerFinal;
using CubeManager.Helpers;
using Wpf.Ui.Controls;

namespace CubeManager.FirstRun;

public partial class WelcomeWindow : UiWindow
{
    private readonly SoundManager _soundManager = new();

    private ConfigManager ConfigManager { get; } = ConfigManager.Instance;

    public WelcomeWindow()
    {
        InitializeComponent();
    }

    private void ChooseButton_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private async void ChooseButton_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);

        var redeemSuccessful =
            await APICalls.RedeemLicense(ConfigManager.Instance.Config.UserData.Token, LicenseBox.Text);
        if (!redeemSuccessful) return;

        var checkSuccessful = await APICalls.CheckLicense(ConfigManager.Instance.Config.UserData.Token);
        if (!checkSuccessful) return;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var finalWindow = new CubeManagerDashboard();
            finalWindow.Show();
            Close();
        });
    }

    private void ContinueButton_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void ContinueButton_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        var finalWindow = new CubeManagerDashboard();
        finalWindow.Show();
        Close();
    }

    private void WelcomeWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private async void WelcomeWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ConfigManager.Instance.Config.UserData.Token == null) return;
        var checkSuccessful = await APICalls.CheckLicense(ConfigManager.Instance.Config.UserData.Token);
        if (!checkSuccessful) return;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var finalWindow = new CubeManagerDashboard();
            finalWindow.Show();
            Close();
        });
    }
}