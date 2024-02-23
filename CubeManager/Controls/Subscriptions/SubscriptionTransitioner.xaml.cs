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

    private void SubscriptionTransitioner_OnLoaded(object sender, RoutedEventArgs e)
    {
       if(_configManager.Config.Subscriptions.Subscriptions.Count == 0)
       {
           ActiveTransitionContent.Navigate(new NewSubscriptionUI());
       }
       else
       {
           ActiveTransitionContent.Navigate(new SubscriptionUI());
       }
    }
}