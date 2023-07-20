namespace CubeManager.Models;

public class SoundSettings
{
    public string ButtonHover { get; set; } = "pack://application:,,,/Resources/hover.wav";
    public string ButtonClick { get; set; } = "pack://application:,,,/Resources/click.wav";
    public string CheckboxOff { get; set; } = "pack://application:,,,/Resources/checkboxOff.wav";
    public string CheckboxOn { get; set; } = "pack://application:,,,/Resources/CheckboxOn.wav";
    public string CreditsGet { get; set; } = "pack://application:,,,/Resources/creditsGet.wav";
    public string TaskComplete { get; set; } = "pack://application:,,,/Resources/success.wav";
}