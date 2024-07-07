using System.Windows;
using System.Windows.Controls;
using CubeManager.Controls.Subscriptions.Models;
using CubeManager.Helpers;

namespace CubeManager.Controls.Subscriptions;

public partial class SubscriptionUI : UserControl
{
    private List<SubscriptionItem> Subscriptions { get; set; }

    public SubscriptionUI()
    {
        InitializeComponent();
    }

    private void LoadSubscriptions()
    {
        Subscriptions = ConfigManager.Instance.Config.Subscriptions.Subscriptions;
    }

    private void SubscriptionUI_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadSubscriptions();
        LoadDetails();
        SubscriptionManager.UpdateSettings();
        SubscriptionsGrid.ItemsSource = Subscriptions;
    }


    private void LoadDetails()
    {
        var text = $"Monthly Cost: {ConfigManager.Instance.Config.Subscriptions.Settings.MonthlyCost}EUR | " +
                   $"Yearly Cost: {ConfigManager.Instance.Config.Subscriptions.Settings.YearlyCost}EUR | " +
                   $"Total Cost: {ConfigManager.Instance.Config.Subscriptions.Settings.TotalCost}EUR | " +
                   $"Total Subscriptions: {Subscriptions.Count}";
        FetailsFooter.Footer = text;
    }

    private void CreateNewSubscriptionButton_OnClick(object sender, RoutedEventArgs e)
    {
        var subscriptionTransitioner = new SubscriptionTransitioner();
        subscriptionTransitioner.ChangeNavigationContent(new NewSubscriptionUI());
    }

    private void EditSubButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void DeleteSubscriptionButton_OnClick(object sender, RoutedEventArgs e)
    {
        var tag = SubscriptionsGrid.Tag;
        var subscriptionInConfig =
            ConfigManager.Instance.Config.Subscriptions.Subscriptions.FirstOrDefault(sub => sub.Id == (Guid)tag);
        if (subscriptionInConfig is not null)
            ConfigManager.Instance.Config.Subscriptions.Subscriptions.Remove(subscriptionInConfig);
        ConfigManager.Instance.UpdateConfig(
            config => config.Subscriptions = ConfigManager.Instance.Config.Subscriptions);
        LoadSubscriptions();
        LoadDetails();
    }
}