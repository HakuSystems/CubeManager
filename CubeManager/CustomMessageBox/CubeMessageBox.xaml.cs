using System.Windows;
using System.Windows.Input;
using CubeManager.Helpers;
using Wpf.Ui.Controls;

namespace CubeManager.CustomMessageBox;

public partial class CubeMessageBox : FluentWindow
{
    private static SoundManager _soundManager = new SoundManager();
    public CubeMessageBox()
    {
        InitializeComponent();
        this.DataContext = DataContext;
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        this.DialogResult = true;
    }

    private void ReportButton_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        throw new NotImplementedException();
    }

    private void OkButton_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void ReportButton_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void TitleBar_OnCloseClicked(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }
}