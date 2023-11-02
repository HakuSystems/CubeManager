using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;
using Wpf.Ui.Controls;

namespace CubeManager.Settings;

public partial class SettingsWindow : UiWindow
{
    private static SettingsWindow instance;

    public SettingsWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     in order to open the window only once we create a singleton
    /// </summary>
    public static SettingsWindow Instance => instance ??= new SettingsWindow();

    private void LoginWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        //MainContentFrame
        if ((bool)SoundSwitch.IsChecked! || (bool)DopamineSwitch.IsChecked!)
            ReadyButtonTransition.Visibility = Visibility.Visible;
        else
            ReadyButtonTransition.Visibility = Visibility.Hidden;
    }

    private void LoginWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void DopamineSwitch_OnChecked(object sender, RoutedEventArgs e)
    {
        var description = DopmaineDescriptionText;
        var title = DopamineTitleText;
        var card = DopamineCard;
        var switcher = DopamineSwitch;
        var icon = DopamineIcon;

        if ((bool)SoundSwitch.IsChecked! || (bool)DopamineSwitch.IsChecked!)
            ReadyButtonTransition.Visibility = Visibility.Visible;
        else
            ReadyButtonTransition.Visibility = Visibility.Hidden;

        ApplyColorAnimation(description, "#5a696f", "#ffffff");
        ApplyColorAnimation(title, "#5a696f", "#ffffff");
        ApplyColorAnimation(card, "#181818", "#292929");
        ApplyColorAnimation(switcher, "#5a696f", "#ffffff");
        ApplyColorAnimation(icon, "#000000", "#ffffff");
    }

    private void DopamineSwitch_OnUnchecked(object sender, RoutedEventArgs e)
    {
        var description = DopmaineDescriptionText;
        var title = DopamineTitleText;
        var card = DopamineCard;
        var switcher = DopamineSwitch;
        var icon = DopamineIcon;

        if ((bool)SoundSwitch.IsChecked! || (bool)DopamineSwitch.IsChecked!)
            ReadyButtonTransition.Visibility = Visibility.Visible;
        else
            ReadyButtonTransition.Visibility = Visibility.Hidden;

        ApplyColorAnimation(description, "#ffffff", "#5a696f");
        ApplyColorAnimation(title, "#ffffff", "#5a696f");
        ApplyColorAnimation(card, "#292929", "#181818");
        ApplyColorAnimation(switcher, "#ffffff", "#5a696f");
        ApplyColorAnimation(icon, "#ffffff", "#000000");
    }

    private void ApplyColorAnimation(FrameworkElement element, string fromColor, string toColor,
        double durationInSeconds = 0.2)
    {
        var colorAnimation = new ColorAnimation();
        colorAnimation.From = (Color)ColorConverter.ConvertFromString(fromColor);
        colorAnimation.To = (Color)ColorConverter.ConvertFromString(toColor);
        colorAnimation.Duration = new Duration(TimeSpan.FromSeconds(durationInSeconds));

        PropertyPath colorTargetPath;
        switch (element)
        {
            case TextBlock:
            case PackIcon:
                colorTargetPath = new PropertyPath("Foreground.Color");
                break;
            case Control:
                colorTargetPath = new PropertyPath("Background.Color");
                break;
            default:
                throw new InvalidOperationException($"Unsupported type {element.GetType()}");
        }

        var sb = new Storyboard();
        Storyboard.SetTarget(colorAnimation, element);
        Storyboard.SetTargetProperty(colorAnimation, colorTargetPath);
        sb.Children.Add(colorAnimation);
        sb.Begin();
    }

    private void DopamineCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        var title = DopamineTitleText;
        var icon = DopamineIcon;
        var switcher = DopamineSwitch;

        if ((bool)switcher.IsChecked!)
            return;

        ApplyColorAnimation(title, "#5a696f", "#ffffff");
        ApplyColorAnimation(icon, "#000000", "#ffffff");
    }

    private void DopamineCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        var title = DopamineTitleText;
        var icon = DopamineIcon;
        var switcher = DopamineSwitch;

        if ((bool)switcher.IsChecked!)
            return;

        ApplyColorAnimation(title, "#ffffff", "#5a696f");
        ApplyColorAnimation(icon, "#ffffff", "#000000");
    }

    private void SoundCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        var title = SoundTitleText;
        var icon = SoundIcon;
        var switcher = SoundSwitch;

        if ((bool)switcher.IsChecked!)
            return;

        ApplyColorAnimation(title, "#5a696f", "#ffffff");
        ApplyColorAnimation(icon, "#000000", "#ffffff");
    }

    private void SoundCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        var title = SoundTitleText;
        var icon = SoundIcon;
        var switcher = SoundSwitch;

        if ((bool)switcher.IsChecked!)
            return;

        ApplyColorAnimation(title, "#ffffff", "#5a696f");
        ApplyColorAnimation(icon, "#ffffff", "#000000");
    }

    private void SoundSwitch_OnUnchecked(object sender, RoutedEventArgs e)
    {
        var description = SoundDescriptionText;
        var title = SoundTitleText;
        var card = SoundCard;
        var switcher = SoundSwitch;
        var icon = SoundIcon;

        if ((bool)SoundSwitch.IsChecked! || (bool)DopamineSwitch.IsChecked!)
            ReadyButtonTransition.Visibility = Visibility.Visible;
        else
            ReadyButtonTransition.Visibility = Visibility.Hidden;

        ApplyColorAnimation(description, "#ffffff", "#5a696f");
        ApplyColorAnimation(title, "#ffffff", "#5a696f");
        ApplyColorAnimation(card, "#292929", "#181818");
        ApplyColorAnimation(switcher, "#ffffff", "#5a696f");
        ApplyColorAnimation(icon, "#ffffff", "#000000");
    }

    private void SoundSwitch_OnChecked(object sender, RoutedEventArgs e)
    {
        var description = SoundDescriptionText;
        var title = SoundTitleText;
        var card = SoundCard;
        var switcher = SoundSwitch;
        var icon = SoundIcon;

        if ((bool)SoundSwitch.IsChecked! || (bool)DopamineSwitch.IsChecked!)
            ReadyButtonTransition.Visibility = Visibility.Visible;
        else
            ReadyButtonTransition.Visibility = Visibility.Hidden;

        ApplyColorAnimation(description, "#5a696f", "#ffffff");
        ApplyColorAnimation(title, "#5a696f", "#ffffff");
        ApplyColorAnimation(card, "#181818", "#292929");
        ApplyColorAnimation(switcher, "#5a696f", "#ffffff");
        ApplyColorAnimation(icon, "#000000", "#ffffff");
    }
}