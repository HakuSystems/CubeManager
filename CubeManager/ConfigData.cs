using CubeManager.Controls.CompactSettings;
using CubeManager.Controls.Subscriptions.Models;
using CubeManager.Controls.Todos.Models;
using CubeManager.LoginRegister;
using CubeManager.Models.ScoreBoard;
using CubeManager.Models.Version;
using CubeManager.Settings;
using CubeManager.ZenQuotes;

namespace CubeManager;

public class ConfigData
{
    public VersionData Version { get; set; } = new();
    public bool IsFirstRun { get; set; } = true;
    public ScoreBoardData ScoreBoard { get; set; } = new();
    public CompactSettingsModel Settings { get; set; } = new();
    public SoundSettings SoundSettings { get; set; } = new();
    public SubscriptionsData Subscriptions { get; set; } = new();
    public List<TodoModel> Todos { get; set; } = new();
    public QuoteData Quote { get; set; } = new();
    public UserData UserData { get; set; } = new();
}
