using CubeManager.Models.ScoreBoard;
using CubeManager.Models.Settings;
using CubeManager.Models.Subscriptions;
using CubeManager.Models.Version;
using CubeManager.Todos;

namespace CubeManager.Models;

public class ConfigData
{
    public VersionData Version { get; set; } = new();
    public ScoreBoardData ScoreBoard { get; set; } = new();
    public SettingsData Settings { get; set; } = new();
    public SoundSettings SoundSettings { get; set; } = new();
    public SubscriptionsData Subscriptions { get; set; } = new();
    public TodoData Todo { get; set; } = new();
}