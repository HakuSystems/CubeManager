using System.Windows;
using System.Windows.Input;
using CubeManager.CubeManagerFinal;
using CubeManager.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Wpf.Ui.Controls;

namespace CubeManager.FirstRun;

public partial class WelcomeWindow : UiWindow
{
    private SoundManager _soundManager = new();

    public WelcomeWindow()
    {
        InitializeComponent();
    }

    private void ChooseButton_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void ChooseButton_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
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
        if(e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}