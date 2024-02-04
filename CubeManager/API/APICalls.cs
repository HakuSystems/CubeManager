using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace CubeManager.API;

public class APICalls
{
    private static string Url { get; set; } = "https://localhost:5001/api/"; //Change later
    private static HttpClient Client { get; set; } = new HttpClient();
    
    public static async Task<BaseResponse?> Login(string username, string password)
    {
        var response = await Client.PostAsync(Url + "login", new StringContent(JsonConvert.SerializeObject(new LoginResponse()
        {
            Username = username,
            Password = password
        }), Encoding.UTF8, "application/json"));
        return JsonConvert.DeserializeObject<BaseResponse>(await response.Content.ReadAsStringAsync());
    }
    public static async Task<BaseResponse?> Login(string token)
    {
        var response = await Client.PostAsync(Url + "login", new StringContent(JsonConvert.SerializeObject(new LoginResponse()
        {
            Token = token
        }), Encoding.UTF8, "application/json"));
        return JsonConvert.DeserializeObject<BaseResponse>(await response.Content.ReadAsStringAsync());
    }
    public static async Task<BaseResponse?> Register(string username, string password, string email)
    {
        var response = await Client.PostAsync(Url + "register", new StringContent(JsonConvert.SerializeObject(new RegisterResponse()
        {
            Username = username,
            Password = password,
            Email = email,
            CheckSum = CheckSumHasher.MD5Hash(username + password + email)
        }), Encoding.UTF8, "application/json"));
        return JsonConvert.DeserializeObject<BaseResponse>(await response.Content.ReadAsStringAsync());
    }
    public static async Task<VersionResponse?> Version()
    {
        var response = await Client.GetAsync(Url + "version");
        return JsonConvert.DeserializeObject<VersionResponse>(await response.Content.ReadAsStringAsync());
    }
    
    public static void Dispose()
    {
        Client.Dispose();
    }
    
}