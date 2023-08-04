using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CubeManager.Controls;
using CubeManager.Helpers;
using CubeManager.Todos;

namespace CubeManager;

public partial class CubeMangerWindow : Window
{
    private readonly Logger _logger;

    private readonly SoundManager _soundManager = new();

    public CubeMangerWindow()
    {
        _logger = new Logger();
        InitializeComponent();
        _logger.PrioInfo("CubeManagerWindow initialized");
    }

    private double CurrentProgressValue
    {
        get => ConfigManager.Instance.Config.ScoreBoard.CurrentProgress;
        set { ConfigManager.Instance.UpdateConfig(config => config.ScoreBoard.CurrentProgress = value); }
    }

    private int CurrentLevelValue
    {
        get => ConfigManager.Instance.Config.ScoreBoard.CurrentLvl;
        set { ConfigManager.Instance.UpdateConfig(config => config.ScoreBoard.CurrentLvl = value); }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        LvlTxtBox.Text = $"LvL: {CurrentLevelValue}";
        LvlProgbar.Value = CurrentProgressValue;
        _logger.Info($"Rendered Level: {CurrentLevelValue}, Progress: {CurrentProgressValue}");
    }

    private void DragGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
    }

    private void MinBtn_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
        _logger.Info("Minimized CubeManagerWindow");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void ClosBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = MessageBox.Show("Are you sure you want to exit? Notifications wont Work.", "Exit",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (dialog == MessageBoxResult.No) return;
        Close();
        _logger.PrioInfo("Closed CubeManagerWindow");
    }


    public void DoLevelUp()
    {
        LvlProgbar.Value += 10;
        if (!LvlProgbar.Value.Equals(100)) return;
        LvlTxtBox.Text = $"LvL: {++CurrentLevelValue}";
        LvlProgbar.Value = 0;
        _logger.Info($"Level updated to: {CurrentLevelValue}");
        LevelUp();
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.TaskComplete);
    }

    private void LevelUp()
    {
        var growAnimation = new DoubleAnimation
        {
            From = 1,
            To = 1.5,
            Duration = new Duration(TimeSpan.FromSeconds(0.2)),
            AutoReverse = true
        };
        LvlTxtBox.RenderTransform = new ScaleTransform();
        LvlTxtBox.RenderTransformOrigin = new Point(0.5, 0.5);
        LvlTxtBox.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, growAnimation);
        LvlTxtBox.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, growAnimation);

        var originalBrush = LvlTxtBox.Foreground as SolidColorBrush;
        var animatableBrush = new SolidColorBrush(originalBrush!.Color);
        LvlTxtBox.Foreground = animatableBrush;

        var colorAnimation = new ColorAnimation
        {
            From = originalBrush.Color,
            To = Colors.Red,
            Duration = new Duration(TimeSpan.FromSeconds(0.4)),
            AutoReverse = true
        };
        animatableBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);


        var spinAnimation = new DoubleAnimation
        {
            From = 0,
            To = 360,
            Duration = new Duration(TimeSpan.FromSeconds(1))
        };
        Sparkles.RenderTransform = new RotateTransform();
        Sparkles.RenderTransformOrigin = new Point(0.5, 0.5);
        Sparkles.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, spinAnimation);


        var flashAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(0.1)),
            AutoReverse = true,
            RepeatBehavior = new RepeatBehavior(5)
        };
        ScoreBoardButton.Opacity = 1;
        ScoreBoardButton.BeginAnimation(OpacityProperty, flashAnimation);
    }

    private void LvlProgbar_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        CurrentProgressValue = e.NewValue;
        _logger.Info($"Progress value changed to: {CurrentProgressValue}");
    }

    private void SettingsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ControlsFrame.Navigate(new SettingsControl());
        _logger.Info("Navigated to SettingsControl");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void DiscordBtn_OnClick(object sender, RoutedEventArgs e)
    {
        //https://discord.gg/7t4MQFKjUM
        Process.Start(new ProcessStartInfo("cmd", "/c start https://discord.gg/7t4MQFKjUM")
        {
            CreateNoWindow = true
        });
        _logger.PrioInfo("Opened Discord link");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void SubscriptionsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ControlsFrame.Navigate(new SubscriptionsControl());
        _logger.Info("Navigated to SubscriptionsControl");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }


    private void TodosBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ControlsFrame.Navigate(new TodoControl());
        _logger.Info("Navigated to TodosControl");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void RoutineBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void RoutineBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void TodosBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void SubscriptionsBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void LifeGoalsBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void LifeGoalsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void BirthdaysBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void BirthdaysBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void FamilyCalendarBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void FamilyCalendarBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void GameReleasesBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void GameReleasesBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void PlayTimeBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void PlayTimeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void SettingsBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void DiscordBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void MinBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void ClosBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void CubeMangerWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        VersionTxtBlock.Text = $"V{Assembly.GetExecutingAssembly().GetName().Version}";
    }
}