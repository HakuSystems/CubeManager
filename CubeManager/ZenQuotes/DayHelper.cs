using CubeManager.Helpers;

namespace CubeManager.ZenQuotes;

public class DayHelper
{
    //basicly only do a api call today and then do the next one next day
    private static readonly FetchQuote FetchQuote = new();
    private static readonly Logger Logger = new();

    public static void SaveQuote(string quote)
    {
        var config = ConfigManager.Instance.Config;
        config.Quote.Quote = quote;
        config.Quote.LastApiCall = DateTime.Now;
        ConfigManager.Instance.UpdateConfig(config => config.Quote = config.Quote);
        Logger.PrioInfo("Quote saved");
    }
}