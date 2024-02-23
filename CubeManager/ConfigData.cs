using CubeManager.Controls.Subscriptions.Models;
using CubeManager.Models.ScoreBoard;
using CubeManager.Models.Version;
using CubeManager.Settings;
using CubeManager.Todos;
using CubeManager.ZenQuotes;

namespace CubeManager;

public class ConfigData
{
    public VersionData Version { get; set; } = new();
    public bool IsFirstRun { get; set; } = true;
    public ScoreBoardData ScoreBoard { get; set; } = new();
    public SettingsData Settings { get; set; } = new();
    public SoundSettings SoundSettings { get; set; } = new();
    public SubscriptionsData Subscriptions { get; set; } = new();
    public TodoData Todo { get; set; } = new();
    public QuoteData Quote { get; set; } = new();
}
