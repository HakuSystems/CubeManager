using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CubeManager.Helpers;
using MaterialDesignThemes.Wpf;

namespace CubeManager.Controls;

public partial class SettingsControl : UserControl
{
    private readonly Logger _logger;
    private readonly Dictionary<Card, Color> originalColors = new();

    public SettingsControl()
    {
        _logger = new Logger();
        InitializeComponent();
    }


    private bool EnableDopamineEffects
    {
        get => ConfigManager.Instance.Config.Settings.EnableDopamineEffects;
        set => ConfigManager.Instance.UpdateConfig(config => config.Settings.EnableDopamineEffects = value);
    }


    private void AnimationMaterialCard(Card card, bool activationStatus)
    {
        var solidColorBrush = card.Background as SolidColorBrush;

        if (activationStatus)
        {
            if (!originalColors.ContainsKey(card))
                originalColors[card] = solidColorBrush.Color; // Store the original color

            var colorAnimation = new ColorAnimation
            {
                From = originalColors[card],
                To = Colors.Green,
                Duration = new Duration(TimeSpan.FromSeconds(0.2)),
                AutoReverse = false
            };

            solidColorBrush = new SolidColorBrush(originalColors[card]);
            card.Background = solidColorBrush;

            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }
        else
        {
            if (!originalColors
                    .ContainsKey(card))
                return; // If there is no stored original color for this card, we can't animate it

            var colorAnimation = new ColorAnimation
            {
                From = Colors.Green,
                To = originalColors[card],
                Duration = new Duration(TimeSpan.FromSeconds(0.2)),
                AutoReverse = false
            };

            solidColorBrush = new SolidColorBrush(Colors.Green);
            card.Background = solidColorBrush;

            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }
    }


    private void DopamineToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        AnimationMaterialCard(DopamineCard, true);
        EnableDopamineEffects = true;
        _logger.Info("Dopamine effects have been enabled.");
    }

    private void DopamineToggle_OnUnchecked(object sender, RoutedEventArgs e)
    {
        AnimationMaterialCard(DopamineCard, false);
        EnableDopamineEffects = false;
        _logger.Info("Dopamine effects have been disabled.");
    }

    private void SettingsControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        DopamineToggle.IsChecked = EnableDopamineEffects;
    }

    private void DopamineCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DopamineToggle.IsChecked = !DopamineToggle.IsChecked;
    }

    private void DialogHostOperation_OnDialogClosed(object sender, DialogClosedEventArgs eventargs)
    {
        DialogHostOperation.IsOpen = false;
    }
}