using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CubeManager.FirstRun;
using CubeManager.Helpers;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
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
        if (EnableDopamineEffects) DopamineSwitch.IsChecked = true;

        if (EnableSound) SoundSwitch.IsChecked = true;

        ReadyButtonTransition.Visibility = EnableDopamineEffects || EnableSound
            ? Visibility.Visible
            : Visibility.Collapsed;
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
        DopamineSwitch.Content = "Enabled";


        _logger.Info("Dopamine effects have been enabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOn);
        ReadyButtonTransition.Visibility = EnableDopamineEffects || EnableSound
            ? Visibility.Visible
            : Visibility.Collapsed;

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
        DopamineSwitch.Content = "Disabled";


        _logger.Info("Dopamine effects have been disabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOff);
        ReadyButtonTransition.Visibility = EnableDopamineEffects || EnableSound
            ? Visibility.Visible
            : Visibility.Collapsed;

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
        SoundSwitch.Content = "Disabled";


        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOff);
        _logger.Info("Sound has been disabled.");
        ReadyButtonTransition.Visibility = EnableDopamineEffects || EnableSound
            ? Visibility.Visible
            : Visibility.Collapsed;

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
        SoundSwitch.Content = "Enabled";

        _logger.Info("Sound has been enabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOn);
        ReadyButtonTransition.Visibility = EnableDopamineEffects || EnableSound
            ? Visibility.Visible
            : Visibility.Collapsed;

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
        ReadyButtonTransition.Visibility = EnableDopamineEffects || EnableSound
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void SoundCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        EnableSound = !EnableSound;
        SoundSwitch.IsChecked = EnableSound;
        ReadyButtonTransition.Visibility = EnableDopamineEffects || EnableSound
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void VolumeSliderDesign_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!EnableSound) return;
        if (VolumeAnimationNormal == null) return;
        VolumeAnimationNormal.Visibility = Visibility.Collapsed;
        VolumeAnimation.Visibility = Visibility.Visible;
        ChangeVolAnimationText.Text = $"{(int)e.NewValue}%";


        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
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
        HoverBadge.Content = "Sound Reset!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            HoverBadge.Content = null;
            timer.Stop();
        };
    }

    private void ResetClickSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentClickSound = "pack://application:,,,/Resources/click.wav";
        ClickBadge.Content = "Sound Reset!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            ClickBadge.Content = null;
            timer.Stop();
        };
    }

    private void ResetLevelSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentLevelSound = "pack://application:,,,/Resources/creditsGet.wav";
        LevelBadge.Content = "Sound Reset!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            LevelBadge.Content = null;
            timer.Stop();
        };
    }

    private void ResetTaskCompleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentTaskCompleteSound = "pack://application:,,,/Resources/success.wav";
        TaskCompleteBadge.Content = "Sound Reset!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            TaskCompleteBadge.Content = null;
            timer.Stop();
        };
    }

    private void ResetCheckboxOffSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentCheckboxOffSound = "pack://application:,,,/Resources/checkboxOff.wav";
        CheckboxOffBadge.Content = "Sound Reset!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            CheckboxOffBadge.Content = null;
            timer.Stop();
        };
    }

    private void ResetCheckboxOnSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CurrentCheckboxOnSound = "pack://application:,,,/Resources/CheckboxOn.wav";
        CheckboxOnBadge.Content = "Sound Reset!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            CheckboxOnBadge.Content = null;
            timer.Stop();
        };
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

    private void DopamineEffectsQuestionmarkBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(CurrentHoverSound);
    }

    private void SettingsQuestionmarkBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(CurrentHoverSound);
    }

    private void ChangeHoverSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentClickSound);
        ChangeSound(CurrentHoverSound, "Hover");
        HoverBadge.Content = "Sound Changed!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            HoverBadge.Content = null;
            timer.Stop();
        };
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
        ClickBadge.Content = "Sound Changed!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            ClickBadge.Content = null;
            timer.Stop();
        };
    }

    private void ChangeLevelSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentLevelSound);
        ChangeSound(CurrentLevelSound, "Level");
        LevelBadge.Content = "Sound Changed!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            LevelBadge.Content = null;
            timer.Stop();
        };
    }

    private void ChangeTaskCompleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentTaskCompleteSound);
        ChangeSound(CurrentTaskCompleteSound, "TaskComplete");
        TaskCompleteBadge.Content = "Sound Changed!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            TaskCompleteBadge.Content = null;
            timer.Stop();
        };
    }

    private void ChangeCheckboxOffSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentCheckboxOffSound);
        ChangeSound(CurrentCheckboxOffSound, "CheckboxOff");
        CheckboxOffBadge.Content = "Sound Changed!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            CheckboxOffBadge.Content = null;
            timer.Stop();
        };
    }

    private void ChangeCheckboxOnSoundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentCheckboxOnSound);
        ChangeSound(CurrentCheckboxOnSound, "CheckboxOn");
        CheckboxOnBadge.Content = "Sound Changed!";
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            CheckboxOnBadge.Content = null;
            timer.Stop();
        };
    }

    private void ReadyBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentClickSound);
        if (ConfigManager.Instance.Config.IsFirstRun)
        {
            var welcomeWindow = new WelcomeWindow();
            welcomeWindow.Show();
        }
        else
        {
            var cubemanagerWindow = new CubeMangerWindow();
            cubemanagerWindow.Show();
        }
        
        Close();
    }

    private void DopamineEffectsQuestionmarkBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentClickSound);
        var customMessageBox = new CustomMessageBox.CubeMessageBox();
        customMessageBox.TitleText.Text = "Dopamine Effects";
        customMessageBox.MessageText.Text =
            "Dopamine effects are visual effects that are shown when you complete a task or level. They are meant to make you feel good about yourself and motivate you to keep going.";
        customMessageBox.ShowDialog();
    }

    private void SettingsQuestionmarkBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(CurrentClickSound);
        var customMessageBox = new CustomMessageBox.CubeMessageBox();
        customMessageBox.TitleText.Text = "Sound";
        customMessageBox.MessageText.Text =
            "Sound allows you to hear sounds when you click on buttons, complete tasks, etc.";
        customMessageBox.ShowDialog();
    }

    private void ReadyBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(CurrentHoverSound);
    }
}