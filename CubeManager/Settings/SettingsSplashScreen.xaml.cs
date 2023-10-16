using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CubeManager.LoginRegister;
using MaterialDesignThemes.Wpf;
using Wpf.Ui.Controls;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace CubeManager.Settings;

public partial class SettingsSplashScreen : UiPage
{
    bool _opened = false;
    public SettingsSplashScreen()
    {
        InitializeComponent();
        Loaded += SettingsSplashScreen_OnLoaded;
    }

    private void SettingsSplashScreen_OnLoaded(object sender, RoutedEventArgs e)
    {
        CreateSettingsRotationIcon();
    }

    private void CreateSettingsRotationIcon()
    {
        var icon = new PackIcon
        {
            Kind = PackIconKind.Settings,
            Foreground = new SolidColorBrush(Colors.White),
            Width = 50,
            Height = 50,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            RenderTransformOrigin = new Point(0.5, 0.5), // Rotate from the center of the icon
            RenderTransform = new TransformGroup
            {
                Children = new TransformCollection
                {
                    new ScaleTransform(),
                    new RotateTransform()
                }
            }
        };

        var scaleAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(0.5))
        };

        var rotateAnimation = new DoubleAnimation
        {
            From = 0,
            To = 360,
            Duration = new Duration(TimeSpan.FromSeconds(3))
        };
        rotateAnimation.Completed += (sender, args) =>
        {
if (_opened) return;
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            _opened = true;
            var loginWindow = (LoginWindow)Window.GetWindow(this);
            loginWindow.Close();
        };

        var scaleTransform =
            (ScaleTransform)((TransformGroup)icon.RenderTransform).Children.First(tr => tr is ScaleTransform);
        var rotateTransform =
            (RotateTransform)((TransformGroup)icon.RenderTransform).Children.First(tr => tr is RotateTransform);

        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        MainCanvas.Children.Add(icon);
    }
}