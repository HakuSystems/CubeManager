using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CubeManager.Controls.Subscriptions;

public partial class NewSubscriptionUI : UserControl
{
    public NewSubscriptionUI()
    {
        InitializeComponent();
    }

    private void SubscriptionName_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        SubscriptionName.Background = !SubscriptionManager.ValidateString(SubscriptionName.Text)
            ? Brushes.DarkRed
            : Brushes.Transparent;
    }

    private void Period_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        Period.Background = !SubscriptionManager.ValidateNumber(Period.Text) ? Brushes.DarkRed : Brushes.Transparent;
    }

    private void FirstPaymentDate_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        FirstPaymentDate.Background = !SubscriptionManager.ValidateDate(FirstPaymentDate.Text)
            ? Brushes.DarkRed
            : Brushes.Transparent;
    }

    private void NewSubscriptionUI_OnLoaded(object sender, RoutedEventArgs e)
    {
        PeriodType.ItemsSource = Enum.GetValues(typeof(SubscriptionManager.PeriodType));
        PeriodType.SelectedIndex = 2;
    }
}