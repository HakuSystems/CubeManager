using System.Windows;
using System.Windows.Controls;
using CubeManager.Helpers;

namespace CubeManager.Controls.Subscriptions;

public partial class SubscriptionTransitioner : UserControl
{
    private readonly ConfigManager _configManager = ConfigManager.Instance;

    public SubscriptionTransitioner()
    {
        InitializeComponent();
    }
    public void ChangeNavigationContent(UserControl content)
    {
        var settings = ConfigManager.Instance.Config.Subscriptions.Settings;
        settings.ActiveTransitionContent = content.GetType().Name;
        ActiveTransitionContent.Navigate(content, typeof(UserControl));
        ConfigManager.Instance.UpdateConfig(config => { config.Subscriptions.Settings = settings; });
    }

    private void SubscriptionTransitioner_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_configManager.Config.Subscriptions.Subscriptions.Count == 0)
        {
            ActiveTransitionContent.Navigate(new NewSubscriptionUI());
            
            var settings = ConfigManager.Instance.Config.Subscriptions.Settings;
            settings.ActiveTransitionContent = "NewSubscriptionUI";
            ConfigManager.Instance.UpdateConfig(config => { config.Subscriptions.Settings = settings; });
        }
        else
        {
            ActiveTransitionContent.Navigate(new SubscriptionUI());
            
            var settings = ConfigManager.Instance.Config.Subscriptions.Settings;
            settings.ActiveTransitionContent = "SubscriptionUI";
            ConfigManager.Instance.UpdateConfig(config => { config.Subscriptions.Settings = settings; });
        }
    }
}