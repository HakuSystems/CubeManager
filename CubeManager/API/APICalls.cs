using System.Net.Http;
using System.Text;
using CubeManager.CustomMessageBox;
using CubeManager.Helpers;
using Newtonsoft.Json;
using Wpf.Ui.Common;

namespace CubeManager.API;

public class APICalls
{
    private static string Url { get; } = "https://cubemanager.zkwolf.com/api/v1/";
    private static HttpClient Client { get; } = new();

    private static async Task<HttpResponseMessage> MakeApiCall(HttpRequestMessage request)
    {
        var response = await Client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var customMessageBoxWindow = new CubeMessageBox
            {
                TitleText = { Text = "API CALL Error" },
                MessageText = { Text = errorContent ?? "An error occurred while logging in. Please try again later." }
            };

            customMessageBoxWindow.ShowDialog();
        }

        return response;
    }


    //login with username and password
    public static async void Login(string username, string password)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new LoginResponse
        {
            Username = username,
            Password = password
        }), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{Url}login"),
            Content = content
        };
        var response = await MakeApiCall(request);
        var result = await response.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<BaseResponse<LoginResponse>>(result);
        
        if (!response.IsSuccessStatusCode)
        {
            var customMessageBoxWindow = new CubeMessageBox
            {
                TitleText = { Text = "LoginUP Error" },
                MessageText = { Text = json?.Message ?? "An error occurred while logging in. Please try again later." }
            };

            customMessageBoxWindow.ShowDialog();
            return;
        }

        ConfigManager.Instance.UpdateConfig(config =>
        {
            config.UserData.Token = json.Data.Token;
            config.UserData.Username = json.Data.Username;
            config.UserData.IsBanned = json.Data.IsBanned;
            config.UserData.UserLevel = json.Data.UserLevel;
        });
    }

    //login with AuthToken
    public static async void Login(string token)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new LoginResponse
        {
            Token = token
        }), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{Url}login"),
            Content = content
        };
        var response = await MakeApiCall(request);
        var result = await response.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<BaseResponse<LoginResponse>>(result);

        if (!response.IsSuccessStatusCode)
        {
            var customMessageBoxWindow = new CubeMessageBox
            {
                TitleText = { Text = "LoginT Error" },
                MessageText = { Text = json?.Message ?? "An error occurred while logging in. Please try again later." }
            };

            customMessageBoxWindow.ShowDialog();
            return;
        }

        ConfigManager.Instance.UpdateConfig(config =>
        {
            config.UserData.Username = json.Data.Username;
            config.UserData.IsBanned = json.Data.IsBanned;
            config.UserData.UserLevel = json.Data.UserLevel;
        });
    }

    //register
    public static async void Register(string username, string password, string email)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new RegisterResponse
        {
            Username = username,
            Password = password,
            Email = email,
            CheckSum = CheckSumHasher.MD5Hash(email + password + username)
        }), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Url + "register"),
            Content = content
        };
        var response = await MakeApiCall(request);
        var result = await response.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<BaseResponse<RegisterResponse>>(result);

        if (!response.IsSuccessStatusCode)
        {
            var customMessageBoxWindow = new CubeMessageBox
            {
                TitleText = { Text = "Register Error" },
                MessageText = { Text = json?.Message ?? "An error occurred while registering. Please try again later." }
            };

            customMessageBoxWindow.ShowDialog();
            return;
        }

        ConfigManager.Instance.UpdateConfig(config => { config.UserData.Token = json.Data.Token; });
        var customMessageBoxWindow2 = new CubeMessageBox
        {
            TitleText = { Text = "Success" },
            MessageText = { Text = "You have successfully registered." }
        };

        customMessageBoxWindow2.ShowDialog();
    }

    // version request
    public static async Task<VersionResponse?> Version()
    {
        var response = await Client.GetAsync(Url + "version");
        return response.IsSuccessStatusCode
            ? JsonConvert.DeserializeObject<VersionResponse>(await response.Content.ReadAsStringAsync())
            : null;
    }

    // /check_license (token == login AuthToken from user)
    public static async void CheckLicense(string token)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new LicenseResponse
        {
            Token = token
        }), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{Url}check_license"),
            Content = content
        };
        var response = await MakeApiCall(request);
        var result = await response.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<BaseResponse<LicenseResponse>>(result);

        if (response.IsSuccessStatusCode) return;
        var customMessageBoxWindow = new CubeMessageBox
        {
            TitleText = { Text = "License Error" },
            MessageText =
                { Text = json?.Message ?? "An error occurred while checking the license. Please try again later." }
        };

        customMessageBoxWindow.ShowDialog();
    }

    // /redeem (token == login AuthToken from user)
    public static async void RedeemLicense(string token, string licenseKey)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new LicenseResponse
        {
            Token = token,
            LicenseKey = licenseKey
        }), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{Url}redeem"),
            Content = content
        };
        var response = await MakeApiCall(request);
        var result = await response.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<BaseResponse<LicenseResponse>>(result);

        if (response.IsSuccessStatusCode) return;
        var customMessageBoxWindow = new CubeMessageBox
        {
            TitleText = { Text = "Redeem Error" },
            MessageText =
                { Text = json?.Message ?? "An error occurred while redeeming the license. Please try again later." }
        };

        customMessageBoxWindow.ShowDialog();
    }

    public static void Dispose()
    {
        Client.Dispose();
    }
}