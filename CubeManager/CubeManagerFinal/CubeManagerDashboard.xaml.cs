using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CubeManager.Controls;
using CubeManager.Controls.Subscriptions;
using CubeManager.Helpers;
using CubeManager.Settings;
using CubeManager.Todos;
using CubeManager.ZenQuotes;
using MaterialDesignThemes.Wpf;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Wpf.Ui.Controls;

namespace CubeManager.CubeManagerFinal;

public partial class CubeManagerDashboard : UiWindow
{
    private readonly Logger _logger;

    private readonly SoundManager _soundManager = new();

    private SKPoint _mousePosition;


    public CubeManagerDashboard()
    {
        _logger = new Logger();
        InitializeComponent();
        _logger.Info("CubeManagerWindow initialized");
        IsFirstRunValue = false;
    }

    private bool IsFirstRunValue
    {
        get => ConfigManager.Instance.Config.IsFirstRun;
        set { ConfigManager.Instance.UpdateConfig(config => config.IsFirstRun = value); }
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

    public void DoLevelUp()
    {
        var random = new Random();
        LvlProgbar.Value += random.Next(0, 11);
    
        if (LvlProgbar.Value >= 100)
        {
            LvlTxtBox.Text = $"LvL: {++CurrentLevelValue}";
            LvlProgbar.Value = 0;
            _logger.Info($"Level updated to: {CurrentLevelValue}");
            LevelUp();
            _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.TaskComplete);
        }
    
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

    private void LvlProgbar_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        CurrentProgressValue = e.NewValue;
        _logger.Info($"Progress value changed to: {CurrentProgressValue}");
    }

    private void SettingsBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void SettingsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _logger.Info("Navigated to SettingsControl");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);

        var settingsWindow = new SettingsWindow();
        settingsWindow.InitializeComponent();
        settingsWindow.ShowDialog();

        var thisWindow = (CubeManagerDashboard)GetWindow(this);
        thisWindow.Close();
    }

    private void DiscordBtn_OnClick(object sender, RoutedEventArgs e)
    {
        //https://discord.gg/7t4MQFKjUM
        Process.Start(new ProcessStartInfo("cmd", "/c start https://discord.gg/nanosdk-607674690655485993")
        {
            CreateNoWindow = true
        });
        _logger.Info("Opened Discord link");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void DiscordBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void RoutineCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);

        ApplyColorAnimation(RoutineCard, "#181818", "#292929");
        ApplyColorAnimation(RoutineTasksIcon, "#000000", "#ffffff");
        ApplyColorAnimation(RoutineTasksText, "#5a696f", "#ffffff");
        RoutineTasksText.Visibility = Visibility.Visible;
    }

    private void RoutineCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void TodoCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        NavigationFrame.Navigate(new TodoControl());
        _logger.Info("Navigated to TodoControl");
    }

    private void TodoCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
        ApplyColorAnimation(TodoCard, "#181818", "#292929");
        ApplyColorAnimation(TodoTasksIcon, "#000000", "#ffffff");
        ApplyColorAnimation(TodoTasksText, "#5a696f", "#ffffff");
        TodoTasksText.Visibility = Visibility.Visible;
    }

    private void SubscriptionsCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        NavigationFrame.Navigate(new SubscriptionTransitioner());
    }

    private void SubscriptionsCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
        ApplyColorAnimation(SubscriptionsCard, "#181818", "#292929");
        ApplyColorAnimation(SubscriptionsIcon, "#000000", "#ffffff");
        ApplyColorAnimation(SubscriptionsText, "#5a696f", "#ffffff");
        SubscriptionsText.Visibility = Visibility.Visible;
    }

    private void LifeGoalsCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void LifeGoalsCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
        ApplyColorAnimation(LifeGoalsCard, "#181818", "#292929");
        ApplyColorAnimation(LifeGoalsIcon, "#000000", "#ffffff");
        ApplyColorAnimation(LifeGoalsText, "#5a696f", "#ffffff");
        LifeGoalsText.Visibility = Visibility.Visible;
    }

    private void BirthdaysCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void BirthdaysCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
        ApplyColorAnimation(BirthdaysCard, "#181818", "#292929");
        ApplyColorAnimation(BirthdaysIcon, "#000000", "#ffffff");
        ApplyColorAnimation(BirthdaysText, "#5a696f", "#ffffff");
        BirthdaysText.Visibility = Visibility.Visible;
    }

    private void FamilyCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void FamilyCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
        ApplyColorAnimation(FamilyCard, "#181818", "#292929");
        ApplyColorAnimation(FamilyIcon, "#000000", "#ffffff");
        ApplyColorAnimation(FamilyText, "#5a696f", "#ffffff");
        FamilyText.Visibility = Visibility.Visible;
    }

    private void GamesCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void GamesCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
        ApplyColorAnimation(GamesCard, "#181818", "#292929");
        ApplyColorAnimation(GamesIcon, "#000000", "#ffffff");
        ApplyColorAnimation(GamesText, "#5a696f", "#ffffff");
        GamesText.Visibility = Visibility.Visible;
    }

    private void PlayTimeCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
    }

    private void PlayTimeCard_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
        ApplyColorAnimation(PlayTimeCard, "#181818", "#292929");
        ApplyColorAnimation(PlayTimeIcon, "#000000", "#ffffff");
        ApplyColorAnimation(PlayTimeText, "#5a696f", "#ffffff");
        PlayTimeText.Visibility = Visibility.Visible;
    }

    private void RoutineCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ApplyColorAnimation(RoutineCard, "#292929", "#181818");
        ApplyColorAnimation(RoutineTasksIcon, "#ffffff", "#000000");
        ApplyColorAnimation(RoutineTasksText, "#ffffff", "#5a696f");
        RoutineTasksText.Visibility = Visibility.Collapsed;
    }

    private void TodoCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ApplyColorAnimation(TodoCard, "#292929", "#181818");
        ApplyColorAnimation(TodoTasksIcon, "#ffffff", "#000000");
        ApplyColorAnimation(TodoTasksText, "#ffffff", "#5a696f");
        TodoTasksText.Visibility = Visibility.Collapsed;
    }

    private void SubscriptionsCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ApplyColorAnimation(SubscriptionsCard, "#292929", "#181818");
        ApplyColorAnimation(SubscriptionsIcon, "#ffffff", "#000000");
        ApplyColorAnimation(SubscriptionsText, "#ffffff", "#5a696f");
        SubscriptionsText.Visibility = Visibility.Collapsed;
    }

    private void LifeGoalsCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ApplyColorAnimation(LifeGoalsCard, "#292929", "#181818");
        ApplyColorAnimation(LifeGoalsIcon, "#ffffff", "#000000");
        ApplyColorAnimation(LifeGoalsText, "#ffffff", "#5a696f");
        LifeGoalsText.Visibility = Visibility.Collapsed;
    }

    private void BirthdaysCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ApplyColorAnimation(BirthdaysCard, "#292929", "#181818");
        ApplyColorAnimation(BirthdaysIcon, "#ffffff", "#000000");
        ApplyColorAnimation(BirthdaysText, "#ffffff", "#5a696f");
        BirthdaysText.Visibility = Visibility.Collapsed;
    }

    private void FamilyCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ApplyColorAnimation(FamilyCard, "#292929", "#181818");
        ApplyColorAnimation(FamilyIcon, "#ffffff", "#000000");
        ApplyColorAnimation(FamilyText, "#ffffff", "#5a696f");
        FamilyText.Visibility = Visibility.Collapsed;
    }

    private void GamesCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ApplyColorAnimation(GamesCard, "#292929", "#181818");
        ApplyColorAnimation(GamesIcon, "#ffffff", "#000000");
        ApplyColorAnimation(GamesText, "#ffffff", "#5a696f");
        GamesText.Visibility = Visibility.Collapsed;
    }

    private void PlayTimeCard_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ApplyColorAnimation(PlayTimeCard, "#292929", "#181818");
        ApplyColorAnimation(PlayTimeIcon, "#ffffff", "#000000");
        ApplyColorAnimation(PlayTimeText, "#ffffff", "#5a696f");
        PlayTimeText.Visibility = Visibility.Collapsed;
    }

    private void ScoreBoardButton_OnClick(object sender, RoutedEventArgs e)
    {
        DoLevelUp();
        //todo: actually implement this
    }

    private void CubeManagerDashboard_OnLoaded(object sender, RoutedEventArgs e)
    {
        ZenquouteText.Text = new FetchQuote().RetrieveQuote();
        ZenquouteTextAuthor.Text = $"- {new FetchQuote().RetrieveQuoteAuthor()}";
    }

    private void CanvasMouseView_OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        using (var paint = new SKPaint())
        {
            paint.Color = RandomColorGenerator.GenerateColorWithAlpha(125).ToSKColor();
            paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 80);
            paint.BlendMode = SKBlendMode.Plus;

            var glowRect = new SKRect(_mousePosition.X - 60, _mousePosition.Y - 50, _mousePosition.X + 50,
                _mousePosition.Y + 50);
            canvas.DrawRect(glowRect, paint);
        }
    }
}