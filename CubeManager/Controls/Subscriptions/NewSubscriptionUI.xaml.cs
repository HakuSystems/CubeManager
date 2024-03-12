using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CubeManager.Helpers;

namespace CubeManager.Controls.Subscriptions;

public partial class NewSubscriptionUI : UserControl
{
    public NewSubscriptionUI()
    {
        InitializeComponent();
    }

    private void SubscriptionName_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        SubscriptionName.Background = !InputChecker.ValidateString(SubscriptionName.Text)
            ? Brushes.DarkRed
            : Brushes.Transparent;
    }

    private void Period_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        Period.Background = !InputChecker.ValidateNumber(Period.Text) ? Brushes.DarkRed : Brushes.Transparent;
    }

    private void FirstPaymentDate_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        FirstPaymentDate.Background = !InputChecker.ValidateDate(FirstPaymentDate.Text)
            ? Brushes.DarkRed
            : Brushes.Transparent;
    }

    private void NewSubscriptionUI_OnLoaded(object sender, RoutedEventArgs e)
    {
        PeriodType.ItemsSource = Enum.GetValues(typeof(SubscriptionManager.PeriodType));
        PeriodType.SelectedIndex = 2;
    }

    private void PiceBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if(InputChecker.ValidatePrice(PiceBox.Text))
        {
            PiceBox.Background = Brushes.Transparent;
        }
        else
        {
            PiceBox.Background = Brushes.DarkRed;
        }
    }

    private void CreateSubBtn_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}