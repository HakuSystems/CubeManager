using System.Net.Http;
using CubeManager.Helpers;
using Newtonsoft.Json;

namespace CubeManager.ZenQuotes;

public class FetchQuote
{
    public string RetrieveQuote()
    {
        if (!IsNewDay() && PingQuoteApiOk()) return ConfigManager.Instance.Config.Quote.Quote;
        var client = new HttpClient();
        var response = client.GetAsync("https://zenquotes.io/api/random").Result;
        var content = response.Content.ReadAsStringAsync().Result;
        var quote = JsonConvert.DeserializeObject<List<QuoteData>>(content).First();
        DayHelper.SaveQuote(quote.q);
        client.Dispose();
        return quote.q;
    }

    private static bool PingQuoteApiOk()
    {
        var client = new HttpClient();
        var response = client.GetAsync("https://zenquotes.io/api/random").Result;
        client.Dispose();
        return response.IsSuccessStatusCode;
    }

    private bool IsNewDay()
    {
        var config = ConfigManager.Instance.Config;
        var lastApiCall = config.Quote.LastApiCall;
        var now = DateTime.Now;
        return lastApiCall.Day != now.Day;
    }
}