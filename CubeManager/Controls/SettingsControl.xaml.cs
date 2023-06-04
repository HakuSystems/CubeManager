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
    private readonly Dictionary<Card, Color> originalColors = new();

    public SettingsControl()
    {
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
        Console.WriteLine("SettingsControl_OnLoaded");
        DopamineToggle.IsChecked = EnableDopamineEffects;
        DebugLogCardCheckBox.IsChecked = EnableDebugConsole;
    }

    private void DopamineCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DopamineToggle.IsChecked = !DopamineToggle.IsChecked;
    }

    private void DebugLogCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DebugLogCardCheckBox.IsChecked = !DebugLogCardCheckBox.IsChecked;
    }

    private void DebugLogCardCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
        AnimationMaterialCard(DebugLogCard, true);
        EnableDebugConsole = true;
    }

    private void DebugLogCardCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
    {
        AnimationMaterialCard(DebugLogCard, false);
        EnableDebugConsole = false;
    }
}