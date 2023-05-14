using System.Diagnostics;
using System.IO;
using CubeManager.Models;
using Newtonsoft.Json;

namespace CubeManager.Helpers;

public class ConfigManager
{
    private static readonly string ConfigFolderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CubeManager");

    private static readonly string ConfigFilePath = Path.Combine(ConfigFolderPath, "config.json");

    private readonly FileSystemWatcher fileWatcher;

    public ConfigManager()
    {
        LoadConfig();

        // Create and configure the FileSystemWatcher to monitor the config file
        fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(ConfigFilePath))
        {
            Filter = Path.GetFileName(ConfigFilePath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
        };

        fileWatcher.Changed += OnConfigFileChanged;

        fileWatcher.EnableRaisingEvents = true;
    }

    public ConfigData Config { get; private set; }

    private async void OnConfigFileChanged(object sender, FileSystemEventArgs e)
    {
        await Task.Delay(100);
        LoadConfig();
    }

    private void LoadConfig()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                Config = JsonConvert.DeserializeObject<ConfigData>(json);
            }
            else
            {
                Config = new ConfigData();
                SaveConfig();
            }
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Could not load config: {ex.Message}");
        }
    }

    private void SaveConfig()
    {
        try
        {
            Directory.CreateDirectory(ConfigFolderPath);
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
            Debug.WriteLine("Config saved");
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Could not save config: {ex.Message}");
        }
    }

    public void UpdateConfig(Action<ConfigData> updateAction)
    {
        updateAction?.Invoke(Config);
        SaveConfig();
    }
}