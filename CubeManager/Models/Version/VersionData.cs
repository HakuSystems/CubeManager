using System.Reflection;

namespace CubeManager.Models.Version;

public class VersionData
{
    public string CurrentVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
}