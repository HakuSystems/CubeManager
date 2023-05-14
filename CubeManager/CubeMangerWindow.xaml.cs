using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CubeManager.Controls;
using CubeManager.Helpers;

namespace CubeManager;

public partial class CubeMangerWindow : Window
{
    private readonly ConfigManager _configManager = new();

    public CubeMangerWindow()
    {
        InitializeComponent();
    }

    private double CurrentProgressValue
    {
        get => _configManager.Config.ScoreBoard.CurrentProgress;
        set => _configManager.UpdateConfig(config => config.ScoreBoard.CurrentProgress = value);
    }

    private int CurrentLevelValue
    {
        get => _configManager.Config.ScoreBoard.CurrentLvl;
        set => _configManager.UpdateConfig(config => config.ScoreBoard.CurrentLvl = value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        LvlTxtBox.Text = $"LvL: {CurrentLevelValue}";
        LvlProgbar.Value = CurrentProgressValue;
    }

    private void DragGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void MinBtn_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void ClosBtn_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void DebugTestBtn_OnClick(object sender, RoutedEventArgs e)
    {
        LvlProgbar.Value += 10;
        if (!LvlProgbar.Value.Equals(100)) return;
        LvlTxtBox.Text = $"LvL: {++CurrentLevelValue}";
        LvlProgbar.Value = 0;
        LevelUp();
    }

    private void LevelUp()
    {
        if (!_configManager.Config.Settings.EnableDopamineEffects) return;
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
    }

    private void SettingsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ControlsFrame.Navigate(new SettingsControl());
    }

    private void DiscordBtn_OnClick(object sender, RoutedEventArgs e)
    {
        //https://discord.gg/7t4MQFKjUM
        Process.Start(new ProcessStartInfo("cmd", "/c start https://discord.gg/7t4MQFKjUM")
        {
            CreateNoWindow = true
        });
    }
}