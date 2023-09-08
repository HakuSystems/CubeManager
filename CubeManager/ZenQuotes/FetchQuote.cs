using System.Net.Http;
using CubeManager.Helpers;
using Newtonsoft.Json;

namespace CubeManager.ZenQuotes;

public class FetchQuote
{
    public string RetrieveQuote()
    {
        if (!IsNewDay()) return ConfigManager.Instance.Config.Quote.Quote;
        var client = new HttpClient();
        var response = client.GetAsync("https://zenquotes.io/api/random").Result;
        var content = response.Content.ReadAsStringAsync().Result;
        var quote = JsonConvert.DeserializeObject<List<QuoteData>>(content).First();
        DayHelper.SaveQuote(quote.q);
        return quote.q;
    }

    private bool IsNewDay()
    {
        var config = ConfigManager.Instance.Config;
        var lastApiCall = config.Quote.LastApiCall;
        var now = DateTime.Now;
        return lastApiCall.Day != now.Day;
    }
}