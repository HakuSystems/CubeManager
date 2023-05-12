using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using CubeManager.Helpers;
using MaterialDesignColors;

namespace CubeManager;

public partial class CubeMangerWindow : Window
{
    private ConfigManager _configManager = new();

    public CubeMangerWindow()
    {
        InitializeComponent();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        LvlTxtBox.Text = $"LvL: {CurrentLevelValue}";
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
        if (LvlProgbar.Value.Equals(100))
        {
            LvlProgbar.Value = 0;
            LvlTxtBox.Text = $"LvL: {++CurrentLevelValue}";

            LevelUp();

            _configManager.UpdateConfig(config => { config.ScoreBoard.CurrentLvl = CurrentLevelValue; });
        }
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


    private int CurrentLevelValue
    {
        get => _configManager.Config.ScoreBoard.CurrentLvl;
        set => _configManager.UpdateConfig(config => config.ScoreBoard.CurrentLvl = value);
    }
}