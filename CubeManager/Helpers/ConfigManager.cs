using System.IO;
using CubeManager.Models;
using Newtonsoft.Json;

namespace CubeManager.Helpers;

public class ConfigManager
{
    private static readonly string ConfigFolderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CubeManager");

    private static readonly string ConfigFilePath = Path.Combine(ConfigFolderPath, "config.json");
    private static ConfigManager _instance;
    private static readonly object _lock = new();
    private readonly Logger _logger;


    private ConfigManager()
    {
        _logger = new Logger();
        _logger.Info("Initializing ConfigManager");
        LoadConfig();
    }

    public static ConfigManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null) _instance = new ConfigManager();
            }

            return _instance;
        }
    }

    public ConfigData Config { get; private set; }

    private void LoadConfig()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                _logger.Info("Config file read successfully");
                Config = JsonConvert.DeserializeObject<ConfigData>(json);
                _logger.PrioInfo("Config Loaded successfully");
            }
            else
            {
                Config = new ConfigData();
                SaveConfig();
                _logger.PrioInfo("No existing config. A new config created and saved");
            }
        }
        catch (IOException ex)
        {
            _logger.ErrSilent($"Could not load config: {ex.Message}");
        }
    }

    private void SaveConfig()
    {
        try
        {
            Directory.CreateDirectory(ConfigFolderPath);
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
            _logger.PrioInfo("Config saved successfully");
        }
        catch (IOException ex)
        {
            _logger.Error($"Could not save config: {ex.Message}");
        }
    }

    public void UpdateConfig(Action<ConfigData> updateAction)
    {
        _logger.Info("Updating config");
        updateAction?.Invoke(Config);
        SaveConfig();
        _logger.PrioInfo("Config updated and saved");
    }
}