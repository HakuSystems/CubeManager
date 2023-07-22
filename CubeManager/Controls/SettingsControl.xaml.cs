using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CubeManager.Helpers;
using CubeManager.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace CubeManager.Controls;

public partial class SettingsControl : UserControl
{
    private readonly Logger _logger;
    private readonly SoundManager _soundManager = new();
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

    private bool EnableSound
    {
        get => ConfigManager.Instance.Config.Settings.EnableSound;
        set => ConfigManager.Instance.UpdateConfig(config => config.Settings.EnableSound = value);
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

    private void DialogHostOperation_OnDialogClosed(object sender, DialogClosedEventArgs eventargs)
    {
        DialogHostOperation.IsOpen = false;
    }

    #region Dopamine

    private void DopamineToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        AnimationMaterialCard(DopamineCard, true);
        EnableDopamineEffects = true;
        _logger.Info("Dopamine effects have been enabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOn);
    }

    private void DopamineToggle_OnUnchecked(object sender, RoutedEventArgs e)
    {
        AnimationMaterialCard(DopamineCard, false);
        EnableDopamineEffects = false;
        _logger.Info("Dopamine effects have been disabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOff);
    }

    private void SettingsControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        DopamineToggle.IsChecked = EnableDopamineEffects;
        SoundToggle.IsChecked = EnableSound;
    }

    private void DopamineCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DopamineToggle.IsChecked = !DopamineToggle.IsChecked;
    }

    #endregion

    #region Sound

    private void SoundToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        AnimationMaterialCard(SoundCard, true);
        ConfigManager.Instance.UpdateConfig(config => config.Settings.EnableSound = true);
        _logger.Info("Sound has been enabled.");
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOn);
    }

    private void SoundToggle_OnUnchecked(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CheckboxOff);
        AnimationMaterialCard(SoundCard, false);
        ConfigManager.Instance.UpdateConfig(config => config.Settings.EnableSound = false);
        _logger.Info("Sound has been disabled.");
    }

    private void SoundCard_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        SoundToggle.IsChecked = !SoundToggle.IsChecked;
    }


    private readonly Dictionary<string, Action<SoundSettings, string>> SoundOptionActions
        = new()
        {
            { "Hover Sound", (settings, path) => settings.ButtonHover = path },
            { "Click Sound", (settings, path) => settings.ButtonClick = path },
            { "Level Sound", (settings, path) => settings.CreditsGet = path },
            { "Task Complete Sound", (settings, path) => settings.TaskComplete = path },
            { "Checkbox off Sound", (settings, path) => settings.CheckboxOff = path },
            { "Checkbox on Sound", (settings, path) => settings.CheckboxOn = path }
        };

    private void SoundOptionsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedSoundOption = ((ListBoxItem)SoundOptionsListBox.SelectedItem)?.Content.ToString();

        if (SoundOptionActions.TryGetValue(selectedSoundOption, out var action))
        {
            SelectedSoundTxtBlock.Text = $"Selected: {selectedSoundOption}";
            OpenFileDialogAndChangeSoundPath(selectedSoundOption, action);
        }
        else
        {
            SelectedSoundTxtBlock.Text = "No Selected Sound";
        }
    }

    private void OpenFileDialogAndChangeSoundPath(string soundOptionName,
        Action<SoundSettings, string> updateSoundPathAction)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Audio files (*.mp3, *.wav)|*.mp3;*.wav",
            Title = "Select a sound file"
        };

        if (openFileDialog.ShowDialog() != true) return;

        var selectedSoundPath = openFileDialog.FileName;

        // update settings object based on button type
        ConfigManager.Instance.UpdateConfig(config => updateSoundPathAction(config.SoundSettings, selectedSoundPath));
    }

    private void ResetSoundsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonClick);
        ConfigManager.Instance.UpdateConfig(config => { config.SoundSettings = new SoundSettings(); });
    }

    private void ResetSoundsBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.ButtonHover);
    }

    #endregion
}