namespace CubeManager.Controls.Subscriptions.Models;

public class SubscriptionsData
{
    public List<SubscriptionItem> Subscriptions { get; set; } = new();
    public List<SubscriptionsSettings> Settings { get; set; } = new();
}