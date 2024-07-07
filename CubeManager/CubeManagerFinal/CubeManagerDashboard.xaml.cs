using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CubeManager.API;
using CubeManager.Controls.Subscriptions;
using CubeManager.Controls.Todos;
using CubeManager.Helpers;
using CubeManager.LoginRegister;
using CubeManager.Settings;
using CubeManager.ZenQuotes;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic.ApplicationServices;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Wpf.Ui.Controls;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace CubeManager.CubeManagerFinal;

public partial class CubeManagerDashboard : FluentWindow
{
    private readonly Logger _logger;

    private readonly SoundManager _soundManager = new();

    private SKPoint _mousePosition;


    public CubeManagerDashboard()
    {
        DataContext = this;
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

        var colorTargetPath = new PropertyPath("Foreground.Color");
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

    private void ScoreBoardButton_OnClick(object sender, RoutedEventArgs e)
    {
        DoLevelUp();
        //todo: actually implement this
    }

    private void CubeManagerDashboard_OnLoaded(object sender, RoutedEventArgs e)
    {
        ZenquouteText.Text = new FetchQuote().RetrieveQuote();
        ZenquouteTextAuthor.Text = $"- {new FetchQuote().RetrieveQuoteAuthor()}";
        if (ConfigManager.Instance.Config.UserData.Username != null)
            WelcomeText.Text = "Welcome, " + ConfigManager.Instance.Config.UserData.Username;
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

    private async void LogoutBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);

        var logoutSuccessful = await APICalls.Logout();
        if (!logoutSuccessful) return;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        });
    }

    private void LogoutBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    private void SubscriptionsNavView_OnClick(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(new SubscriptionTransitioner());
    }

    private void TodosNavView_OnClick(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(new TodoControl());
    }
}