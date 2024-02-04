namespace CubeManager.API;

public class RegisterResponse
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string CheckSum { get; set; } //md5 hash of email + password + username
}