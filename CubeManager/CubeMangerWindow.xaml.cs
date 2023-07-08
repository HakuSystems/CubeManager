using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CubeManager.Controls;
using CubeManager.Helpers;

namespace CubeManager;

public partial class CubeMangerWindow : Window
{
    private readonly Logger _logger;

    public CubeMangerWindow()
    {
        _logger = new Logger();
        InitializeComponent();
        _logger.Info("CubeManagerWindow initialized");
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
        _logger.Debug("DragGrid MouseDown event");
    }

    private void MinBtn_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
        _logger.Info("Minimized CubeManagerWindow");
    }

    private void ClosBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = MessageBox.Show("Are you sure you want to exit? Notifications wont Work.", "Exit",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (dialog == MessageBoxResult.No) return;
        Close();
        _logger.Info("Closed CubeManagerWindow");
    }


    public void DoLevelUp()
    {
        LvlProgbar.Value += 10;
        _logger.Debug($"DebugTestBtn clicked, Progress updated to: {LvlProgbar.Value}");
        if (!LvlProgbar.Value.Equals(100)) return;
        LvlTxtBox.Text = $"LvL: {++CurrentLevelValue}";
        LvlProgbar.Value = 0;
        _logger.Debug($"Level updated to: {CurrentLevelValue}");
        LevelUp();
    }

    private void LevelUp()
    {
        var growAnimation = new DoubleAnimation
        {
            From = 1,
            To = 1.5,
            Duration = new Duration(TimeSpan.FromSeconds(0.2)),
            AutoReverse = true,
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
            AutoReverse = true,
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
        ScoreBoardButton.BeginAnimation(Button.OpacityProperty, flashAnimation);
    }

    private void LvlProgbar_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        CurrentProgressValue = e.NewValue;
        _logger.Debug($"Progress value changed to: {CurrentProgressValue}");
    }

    private void SettingsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ControlsFrame.Navigate(new SettingsControl());
        _logger.Debug("SettingsControl navigated");
    }

    private void DiscordBtn_OnClick(object sender, RoutedEventArgs e)
    {
        //https://discord.gg/7t4MQFKjUM
        Process.Start(new ProcessStartInfo("cmd", "/c start https://discord.gg/7t4MQFKjUM")
        {
            CreateNoWindow = true
        });
        _logger.Info("Discord link opened");
    }

    private void SubscriptionsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ControlsFrame.Navigate(new SubscriptionsControl());
        _logger.Debug("SubscriptionsControl navigated");
    }


    private void TodosBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ControlsFrame.Navigate(new TodosControl());
        _logger.Debug("TodosControl navigated");
    }
}