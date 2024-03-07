namespace CubeManager.API;

public class LoginResponse
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Token { get; set; } //AuthKey when user is allready existing in database then only use this
    public int UserLevel { get; set; }
    public bool IsBanned { get; set; }
}