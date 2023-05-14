using CubeManager.Models.ScoreBoard;
using CubeManager.Models.Settings;
using CubeManager.Models.Version;

namespace CubeManager.Models;

public class ConfigData
{
    public VersionData Version { get; set; } = new();
    public ScoreBoardData ScoreBoard { get; set; } = new();
    public SettingsData Settings { get; set; } = new();
}