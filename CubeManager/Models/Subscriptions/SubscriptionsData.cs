using System.Windows.Media;

namespace CubeManager.Models.Subscriptions;

public class SubscriptionsData
{
    public List<Subscription> Subscriptions { get; set; } = new();
    public Color BackgroundColor { get; set; }
}

public class Subscription
{
    public Guid Id { get; set; } // 12345678-1234-1234-1234-123456789012
    public string? Title { get; set; } // Netflix
    public string? Description { get; set; } // Optional
    public string? Cost { get; set; } // 9.99
    public string? Currency { get; set; } // EUR
    public int Period { get; set; } // 1
    public string? PeriodType { get; set; } // Month
    public bool IsOneTimePayment { get; set; } // yes no
    public DateTime FirstPaymentDate { get; set; } // today

    public DateTime NextPaymentDate { get; set; } // today + 1 month (Period)
    public Color CardColor { get; set; } // #FF0000
    public bool isNeatCard { get; set; } // yes no
}