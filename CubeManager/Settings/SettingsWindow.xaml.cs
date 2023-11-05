using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CubeManager.Helpers;
using MaterialDesignThemes.Wpf;
using Wpf.Ui.Controls;

namespace CubeManager.Settings;

public partial class SettingsWindow : UiWindow
{
    private readonly Logger _logger;
    private readonly SoundManager _soundManager = new();
    private static SettingsWindow instance;

    public SettingsWindow()
    {
        InitializeComponent();
        _logger = new Logger();
        DataContext = this;
    }

    public string CurrentHoverSound
    {
        get => ConfigManager.Instance.Config.SoundSettings.ButtonHover;
        set => ConfigManager.Instance.UpdateConfig(config => config.SoundSettings.ButtonHover = value);
    }

    public string CurrentClickSound
    {
        get => ConfigManager.Instance.Config.SoundSettings.ButtonClick;
        set => ConfigManager.Instance.UpdateConfig(config => config.SoundSettings.ButtonClick = value);
    }

    public string CurrentLevelSound
    {
        get => ConfigManager.Instance.Config.SoundSettings.CreditsGet;
        set => ConfigManager.Instance.UpdateConfig(config => config.SoundSettings.CreditsGet = value);
    }

    public string CurrentTaskCompleteSound
    {
        get => ConfigManager.Instance.Config.SoundSettings.TaskComplete;
        set => ConfigManager.Instance.UpdateConfig(config => config.SoundSettings.TaskComplete = value);
    }

    public string CurrentCheckboxOnSound
    {
        get => ConfigManager.Instance.Config.SoundSettings.CheckboxOn;
        set => ConfigManager.Instance.UpdateConfig(config => config.SoundSettings.CheckboxOn = value);
    }

    public string CurrentCheckboxOffSound
    {
        get => ConfigManager.Instance.Config.SoundSettings.CheckboxOff;
        set => ConfigManager.Instance.UpdateConfig(config => config.SoundSettings.CheckboxOff = value);
    }

    public bool EnableDopamineEffects //Bound
    {
        get => ConfigManager.Instance.Config.Settings.EnableDopamineEffects;
        set => ConfigManager.Instance.UpdateConfig(config => config.Settings.EnableDopamineEffects = value);
    }

    public bool EnableSound //Bound
    {
        get => ConfigManager.Instance.Config.Settings.EnableSound;
        set => ConfigManager.Instance.UpdateConfig(config => config.Settings.EnableSound = value);
    }

    public float SliderVolume //Bound
    {
        get => ConfigManager.Instance.Config.SoundSettings.Volume;
        set => ConfigManager.Instance.UpdateConfig(config => config.SoundSettings.Volume = value);
    }

    /// <summary>
    ///     in order to open the window only once we create a singleton
    /// </summary>
    public static SettingsWindow Instance => instance ??= new SettingsWindow();

    private void LoginWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (EnableDopamineEffects)
            DopamineSwitch.IsChecked = true;
        if (EnableSound)
            SoundSwitch.IsChecked = true;
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


        _logger.Info("Dopamine effects have been enabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOn);


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


        _logger.Info("Dopamine effects have been disabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOff);

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
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
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
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
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


        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOff);
        _logger.Info("Sound has been disabled.");

        ApplyColorAnimation(description, "#ffffff", "#5a696f");
        ApplyColorAnimation(title, "#ffffff", "#5a696f");
        ApplyColorAnimation(card, "#292929", "#181818");
        ApplyColorAnimation(switcher, "#ffffff", "#5a696f");
        ApplyColorAnimation(icon, "#ffffff", "#000000");

        SoundSettingsExpander.Visibility = Visibility.Collapsed;
        VolumeSliderDesign.Visibility = Visibility.Collapsed;
        VolumeAnimationNormal.Visibility = Visibility.Collapsed;
    }

    private void SoundSwitch_OnChecked(object sender, RoutedEventArgs e)
    {
        var description = SoundDescriptionText;
        var title = SoundTitleText;
        var card = SoundCard;
        var switcher = SoundSwitch;
        var icon = SoundIcon;

        _logger.Info("Sound has been enabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOn);

        ApplyColorAnimation(description, "#5a696f", "#ffffff");
        ApplyColorAnimation(title, "#5a696f", "#ffffff");
        ApplyColorAnimation(card, "#181818", "#292929");
        ApplyColorAnimation(switcher, "#5a696f", "#ffffff");
        ApplyColorAnimation(icon, "#000000", "#ffffff");

        SoundSettingsExpander.Visibility = Visibility.Visible;
        VolumeSliderDesign.Visibility = Visibility.Visible;
        VolumeAnimationNormal.Visibility = Visibility.Visible;
    }

    private void DopamineCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        EnableDopamineEffects = !EnableDopamineEffects;
        DopamineSwitch.IsChecked = EnableDopamineEffects;
    }

    private void SoundCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        EnableSound = !EnableSound;
        SoundSwitch.IsChecked = EnableSound;
    }

    private void VolumeSliderDesign_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!EnableSound) return;
        if (VolumeAnimationNormal == null) return;
        VolumeAnimationNormal.Visibility = Visibility.Collapsed;
        VolumeAnimation.Visibility = Visibility.Visible;
        ChangeVolAnimationText.Text = $"{(int)e.NewValue}%";

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            if (VolumeSliderDesign.IsMouseOver)
                return;
            VolumeAnimation.Visibility = Visibility.Collapsed;
            VolumeAnimationNormal.Visibility = Visibility.Visible;
            timer.Stop();
        };
    }

    private void ResetHoverSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentHoverSound = "pack://application:,,,/Resources/hover.wav";
    }

    private void ResetClickSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentClickSound = "pack://application:,,,/Resources/click.wav";
    }

    private void ResetLevelSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentLevelSound = "pack://application:,,,/Resources/creditsGet.wav";
    }

    private void ResetTaskCompleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentTaskCompleteSound = "pack://application:,,,/Resources/success.wav";
    }

    private void ResetCheckboxOffSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentCheckboxOffSound = "pack://application:,,,/Resources/checkboxOff.wav";
    }

    private void ResetCheckboxOnSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentCheckboxOnSound = "pack://application:,,,/Resources/CheckboxOn.wav";
    }

    private void PlayCheckboxOnSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentCheckboxOnSound);
    }

    private void PlayCheckboxOffSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentCheckboxOffSound);
    }

    private void PlayTaskCompleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentTaskCompleteSound);
    }

    private void PlayLevelSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentLevelSound);
    }

    private void PlayClickSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentClickSound);
    }

    private void PlayHoverSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentHoverSound);
    }
}