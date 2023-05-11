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

    private async void DebugTestBtn_OnClick(object sender, RoutedEventArgs e)
    {
        LvlProgbar.Value+=10;
        if (LvlProgbar.Value.Equals(100))
        {
            LvlProgbar.Value = 0;
            LvlTxtBox.Text = $"LvL: {++CurrentLevelValue}";

            
            LvlBtnContent.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5)));
            await Task.Delay(500);
            LvlBtnContent.Effect = new DropShadowEffect()
            {
                Color = Colors.White,
                Direction = 0,
                ShadowDepth = 1,
                Opacity = 1,
                BlurRadius = 10
            };
            LvlBtnContent.BorderThickness = new Thickness(0,0,0,2);
            LvlBtnContent.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5)));
            await Task.Delay(1000);
            LvlBtnContent.BorderThickness = new Thickness(0);
            LvlBtnContent.Effect = null;

            _configManager.UpdateConfig(config =>
            {
                config.ScoreBoard.CurrentLvl = CurrentLevelValue;
            });
            
        }
        
    }

    private int CurrentLevelValue
    {
        get => _configManager.Config.ScoreBoard.CurrentLvl;
        set => _configManager.UpdateConfig(config => config.ScoreBoard.CurrentLvl = value);
    }
}
