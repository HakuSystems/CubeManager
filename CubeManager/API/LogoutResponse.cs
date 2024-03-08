using CubeManager.Helpers;

namespace CubeManager.API;

public class LogoutResponse
{
    public string? Token { get; set; } = ConfigManager.Instance.Config.UserData.Token;
    public bool Force { get; set; } = true;
}