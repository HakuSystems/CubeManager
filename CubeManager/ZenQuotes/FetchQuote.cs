using System.Net.Http;
using CubeManager.Helpers;
using Newtonsoft.Json;

namespace CubeManager.ZenQuotes;

public class FetchQuote
{
    /// <summary>
    ///   Fetches a quote from the ZenQuotes API
    ///   If the quote is too long, it will try again
    ///   If the API is down, it will use the last quote
    /// </summary>
    public string RetrieveQuote()
    {
        var maxChars = 98;
        if (!IsNewDay() && PingQuoteApiOk()) return ConfigManager.Instance.Config.Quote.Quote;
        var client = new HttpClient();
        var response = client.GetAsync("https://zenquotes.io/api/random").Result;
        var content = response.Content.ReadAsStringAsync().Result;
        var quote = JsonConvert.DeserializeObject<List<QuoteData>>(content).First();
        var author = quote.a;
        SaveAuthor(author);
        if (quote.q.Length > maxChars)
        {
            return RetrieveQuote();
        }
        DayHelper.SaveQuote(quote.q);
        client.Dispose();
        return quote.q;
    }

    private void SaveAuthor(string author)
    {
        ConfigManager.Instance.UpdateConfig(config => config.Quote.Author = author);
    }

    public string RetrieveQuoteAuthor()
    {
        return ConfigManager.Instance.Config.Quote.Author;
    }

    /// <summary>
    ///  Checks if the API is up
    /// </summary>
    /// <returns></returns>
    private static bool PingQuoteApiOk()
    {
        var client = new HttpClient();
        var response = client.GetAsync("https://zenquotes.io/api/random").Result;
        client.Dispose();
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    ///  Checks if it is a new day
    /// </summary>
    /// <returns></returns>
    private bool IsNewDay()
    {
        var config = ConfigManager.Instance.Config;
        var lastApiCall = config.Quote.LastApiCall;
        var now = DateTime.Now;
        return lastApiCall.Day != now.Day;
    }
}