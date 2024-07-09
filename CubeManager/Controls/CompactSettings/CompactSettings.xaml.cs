using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CubeManager.CubeManagerFinal;
using CubeManager.CustomMessageBox;
using CubeManager.FirstRun;
using CubeManager.Helpers;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace CubeManager.Controls.CompactSettings;

public partial class CompactSettings : UserControl
{
    private readonly Logger _logger;
    private readonly SoundManager _soundManager = new();
    public CompactSettings()
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
    
    

    private void CompactSettings_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (EnableDopamineEffects) DopamineSwitch.IsChecked = true;
        if (EnableSound) SoundSwitch.IsChecked = true; 
        CurrentVolumeText.Text = $"{(int)SliderVolume}% of the overall volume.";
    }

    private void DopamineSwitch_OnChecked(object sender, RoutedEventArgs e)
    {
        EnableDopamineEffects = true;
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOn);
        
        var description = DopmaineDescriptionText;
        var title = DopamineTitleText;
        var card = DopamineCard;
        var switcher = DopamineSwitch;
        var icon = DopamineIcon;
        
        ApplyColorAnimation(description, "#5a696f", "#ffffff");
        ApplyColorAnimation(title, "#5a696f", "#ffffff");
        ApplyColorAnimation(card, "#181818", "#292929");
        ApplyColorAnimation(switcher, "#5a696f", "#ffffff");
        ApplyColorAnimation(icon, "#000000", "#ffffff");
        
    }

    private void DopamineSwitch_OnUnchecked(object sender, RoutedEventArgs e)
    {
        EnableDopamineEffects = false;
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOff);
        var description = DopmaineDescriptionText;
        var title = DopamineTitleText;
        var card = DopamineCard;
        var switcher = DopamineSwitch;
        var icon = DopamineIcon;
        
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

        _logger.Info("Sound has been enabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOn);

        ApplyColorAnimation(description, "#5a696f", "#ffffff");
        ApplyColorAnimation(title, "#5a696f", "#ffffff");
        ApplyColorAnimation(card, "#181818", "#292929");
        ApplyColorAnimation(switcher, "#5a696f", "#ffffff");
        ApplyColorAnimation(icon, "#000000", "#ffffff");

        SoundSettingsExpander.Visibility = Visibility.Visible;
        VolumeSliderDesign.Visibility = Visibility.Visible;
        VolumeSliderOnEnabled.Visibility = Visibility.Visible;
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
        VolumeSliderOnEnabled.Visibility = Visibility.Collapsed;
    }

    private void SoundCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(CurrentHoverSound);
        var title = SoundTitleText;
        var icon = SoundIcon;
        var switcher = SoundSwitch;

        if ((bool)switcher.IsChecked!)
            return;

        ApplyColorAnimation(title, "#5a696f", "#ffffff");
        ApplyColorAnimation(icon, "#000000", "#ffffff");
        
    }

    private void SetForegroundBrushIfNeeded(FrameworkElement element, string color)
    {
        switch (element)
        {
            case TextBlock textBlock:
                textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                break;
            case PackIcon packIcon:
                packIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                break;
            case Control control:
                control.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                break;
        }
    }
    private void ApplyColorAnimation(FrameworkElement element, string fromColor, string toColor)
    {
        var colorAnimation = new ColorAnimation();
        colorAnimation.From = (Color)ColorConverter.ConvertFromString(fromColor);
        colorAnimation.To = (Color)ColorConverter.ConvertFromString(toColor);
        colorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));

        var colorTargetPath = new PropertyPath("Foreground.Color");
        
        SetForegroundBrushIfNeeded(element, fromColor);
        
        switch (element)
        {
            case TextBlock:
            case PackIcon:
                colorTargetPath = new PropertyPath("Foreground.Color");
                break;
            case Control:
                colorTargetPath = new PropertyPath("Background.Color");
                break;
        }

        var sb = new Storyboard();
        Storyboard.SetTarget(colorAnimation, element);
        Storyboard.SetTargetProperty(colorAnimation, colorTargetPath);
        sb.Children.Add(colorAnimation);
        sb.Begin();
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

    private void DopamineCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(CurrentHoverSound);
        
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

    private void VolumeSliderDesign_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!EnableSound) return;
        CurrentVolumeText.Visibility = Visibility.Visible;
        CurrentVolumeText.Text = $"{(int)e.NewValue}% of the overall volume.";
        VolumeAnimation.Visibility = Visibility.Visible;
        ChangeVolAnimationText.Text = $"{(int)e.NewValue}%";
        SoundIcon.Visibility = Visibility.Collapsed;


        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            if (VolumeSliderDesign.IsMouseOver)
                return;
            VolumeAnimation.Visibility = Visibility.Collapsed;
            CurrentVolumeText.Visibility = Visibility.Visible;
            SoundIcon.Visibility = Visibility.Visible;
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

    private void ChangeHoverSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentClickSound);
        ChangeSound(CurrentHoverSound, "Hover");
    }

    private void ChangeSound(string currentSoundName, string soundName)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "WAV Files (*.wav)|*.wav",
            Title = $"Select {soundName} Sound"
        };
        if (dialog.ShowDialog() != true)
            return;
        var path = dialog.FileName;
        if (path == currentSoundName)
            return;
        switch (soundName)
        {
            case "Hover":
                CurrentHoverSound = path;
                break;
            case "Click":
                CurrentClickSound = path;
                break;
            case "Level":
                CurrentLevelSound = path;
                break;
            case "TaskComplete":
                CurrentTaskCompleteSound = path;
                break;
            case "CheckboxOn":
                CurrentCheckboxOnSound = path;
                break;
            case "CheckboxOff":
                CurrentCheckboxOffSound = path;
                break;
        }
    }

    private void ChangeClickSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentClickSound);
        ChangeSound(CurrentClickSound, "Click");
    }

    private void ChangeLevelSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentLevelSound);
        ChangeSound(CurrentLevelSound, "Level");
    }

    private void ChangeTaskCompleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentTaskCompleteSound);
        ChangeSound(CurrentTaskCompleteSound, "TaskComplete");
    }

    private void ChangeCheckboxOffSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentCheckboxOffSound);
        ChangeSound(CurrentCheckboxOffSound, "CheckboxOff");
    }

    private void ChangeCheckboxOnSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentCheckboxOnSound);
        ChangeSound(CurrentCheckboxOnSound, "CheckboxOn");
    }

    private void DopamineCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DopamineSwitch.IsChecked = !DopamineSwitch.IsChecked;
    }

    private void SoundCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        SoundSwitch.IsChecked = !SoundSwitch.IsChecked;
    }
}