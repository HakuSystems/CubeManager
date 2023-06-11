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

    private bool EnableDebugConsole
    {
        get => ConfigManager.Instance.Config.Settings.EnableDebugConsole;
        set => ConfigManager.Instance.UpdateConfig(config => config.Settings.EnableDebugConsole = value);
    }


    private void AnimationMaterialCard(Card card, bool activationStatus)
    {
        var solidColorBrush = card.Background as SolidColorBrush;

        if (activationStatus)
        {
            _logger.Debug($"Animating card {card.Name} to green");
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
            _logger.Debug($"Card {card.Name} animated to green");
        }
        else
        {
            _logger.Debug($"Animating card {card.Name} to original color");
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
            _logger.Debug($"Card {card.Name} animated to original color");
        }
    }


    private void DopamineToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        _logger.Debug("DopamineToggle_OnChecked");
        AnimationMaterialCard(DopamineCard, true);
        EnableDopamineEffects = true;
    }

    private void DopamineToggle_OnUnchecked(object sender, RoutedEventArgs e)
    {
        _logger.Debug("DopamineToggle_OnUnchecked");
        AnimationMaterialCard(DopamineCard, false);
        EnableDopamineEffects = false;
    }

    private void SettingsControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        _logger.Debug("SettingsControl_OnLoaded");
        DopamineToggle.IsChecked = EnableDopamineEffects;
    }

    private void DopamineCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _logger.Debug("DopamineCard_OnMouseDown");
        DopamineToggle.IsChecked = !DopamineToggle.IsChecked;
    }

    private void ThemeCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _logger.Debug("ThemeCard_OnMouseDown");
        DialogHostOperation.IsOpen = true;
    }

    private void DialogHostOperation_OnDialogClosed(object sender, DialogClosedEventArgs eventargs)
    {
        _logger.Debug("DialogHostOperation_OnDialogClosed");
        DialogHostOperation.IsOpen = false;
    }
}