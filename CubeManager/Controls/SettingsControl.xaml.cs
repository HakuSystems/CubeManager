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
    private readonly ConfigManager _configManager = new();
    private readonly Dictionary<Card, Color> originalColors = new();

    public SettingsControl()
    {
        InitializeComponent();
    }


    private bool EnableDopamineEffects
    {
        get => _configManager.Config.Settings.EnableDopamineEffects;
        set => _configManager.UpdateConfig(config => config.Settings.EnableDopamineEffects = value);
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
    }

    private void DopamineToggle_OnUnchecked(object sender, RoutedEventArgs e)
    {
        AnimationMaterialCard(DopamineCard, false);
        EnableDopamineEffects = false;
    }

    private void SettingsControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        DopamineToggle.IsChecked = EnableDopamineEffects;
    }

    private void DopamineCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DopamineToggle.IsChecked = !DopamineToggle.IsChecked;
    }
}