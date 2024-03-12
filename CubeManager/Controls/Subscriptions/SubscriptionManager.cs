using System.Windows.Media;
using CubeManager.Controls.Subscriptions.Models;
using CubeManager.Helpers;

namespace CubeManager.Controls.Subscriptions;

public class SubscriptionManager
{
    //create a new subscription
    public static void CreateSubscription(string title, string description, string cost, string currency, int period, string periodType, bool isOneTimePayment, DateTime firstPaymentDate, Color cardColor, string paymentMethod = "", string notes = "")   
    {
        var subscription = new SubscriptionItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Cost = cost,
            Currency = currency,
            Period = period,
            PeriodType = periodType,
            IsOneTimePayment = isOneTimePayment,
            FirstPaymentDate = firstPaymentDate,
            NextPaymentDate = firstPaymentDate.AddMonths(period),
            CardColor = cardColor,
            PaymentMethod = paymentMethod,
            Notes = notes
        };
        ConfigManager.Instance.UpdateConfig(config =>
        {
            config.Subscriptions.Subscriptions.Add(subscription);
        });
        UpdateSettings();
    }

    private static void UpdateSettings()
    {
        var settings = ConfigManager.Instance.Config.Subscriptions.Settings;

        settings.MonthlyCost = CalculateMonthlyCost();
        settings.YearlyCost = CalculateYearlyCost();
        settings.TotalCost = CalculateTotalCost();
        settings.SubscriptionsCount = ConfigManager.Instance.Config.Subscriptions.Subscriptions.Count;
        settings.SubscriptionsCountWithOneTimePayment = ConfigManager.Instance.Config.Subscriptions.Subscriptions.Count(x => x.IsOneTimePayment);
        settings.SubscriptionsCountWithRecurringPayment = ConfigManager.Instance.Config.Subscriptions.Subscriptions.Count(x => !x.IsOneTimePayment);
        settings.SubscriptionsCountWithNextPaymentDateInNextMonth = ConfigManager.Instance.Config.Subscriptions.Subscriptions.Count(x => x.NextPaymentDate.Month == DateTime.Now.AddMonths(1).Month);
    
        ConfigManager.Instance.UpdateConfig(config =>
        {
            config.Subscriptions.Settings = settings;
        });
    }

    private static double CalculateYearlyCost()
    {
        var activeSubscriptions = ConfigManager.Instance.Config.Subscriptions.Subscriptions.Where(x => x.NextPaymentDate > DateTime.Now);
        return activeSubscriptions.Sum(x => x.IsOneTimePayment ? double.Parse(x.Cost) : double.Parse(x.Cost) * 12);
    }

    private static double CalculateTotalCost()
    {
        var activeSubscriptions = ConfigManager.Instance.Config.Subscriptions.Subscriptions.Where(x => x.NextPaymentDate > DateTime.Now);
        return activeSubscriptions.Sum(x => double.Parse(x.Cost));
    }

    private static double CalculateMonthlyCost()
    {
        var activeSubscriptions = ConfigManager.Instance.Config.Subscriptions.Subscriptions.Where(x => x.NextPaymentDate > DateTime.Now);
        return activeSubscriptions.Sum(x => x.IsOneTimePayment ? 0 : double.Parse(x.Cost));
    }
    
}