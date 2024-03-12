using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CubeManager.CustomMessageBox;
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
        PeriodType.ItemsSource = Enum.GetValues(typeof(PeriodTypeValue));
        PeriodType.SelectedIndex = 2;
        CurrencyComboBox.SelectedIndex = 0;
    }

    private void PiceBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        PriceBox.Background = InputChecker.ValidatePrice(PriceBox.Text) ? Brushes.Transparent : Brushes.DarkRed;
    }
    private void CreateSubBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (InputChecker.ValidateString(SubscriptionName.Text) && InputChecker.ValidateNumber(Period.Text) &&
            InputChecker.ValidateDate(FirstPaymentDate.Text) && InputChecker.ValidatePrice(PriceBox.Text))
        {
            var periodType = (PeriodTypeValue) PeriodType.SelectedItem;
            var color = ColorPicker.Color;
            SubscriptionManager.CreateSubscription(SubscriptionName.Text, Description.Text, PriceBox.Text,
                CurrencyComboBox.Text, int.Parse(Period.Text), periodType.ToString(), IsOneTimePayment.IsChecked ?? false,
                FirstPaymentDate.SelectedDate ?? DateTime.Now, color);
            var customMessageBoxWindow = new CubeMessageBox
            {
                TitleText = {Text = "Success"},
                MessageText = {Text = "Subscription created successfully"}
            };

            customMessageBoxWindow.ShowDialog();
        }
        else
        {
            var customMessageBoxWindow = new CubeMessageBox
            {
                TitleText = {Text = "Error"},
                MessageText = {Text = "Please fill all fields correctly"}
            };

            customMessageBoxWindow.ShowDialog();
        }
    }
    public enum PeriodTypeValue
    {
        Day,
        Week,
        Month,
        Year
    }

    private void IsOneTimePayment_OnChecked(object sender, RoutedEventArgs e)
    {
        PeriodType.IsEnabled = !(IsOneTimePayment.IsChecked ?? false);
        Period.IsEnabled = !(IsOneTimePayment.IsChecked ?? false);
        BillingPeriodTextBlock.Text = IsOneTimePayment.IsChecked ?? false ? "First payment date" : "Billing date";
    }

    private void IsOneTimePayment_OnUnchecked(object sender, RoutedEventArgs e)
    {
        PeriodType.IsEnabled = !(IsOneTimePayment.IsChecked ?? false);
        Period.IsEnabled = !(IsOneTimePayment.IsChecked ?? false);
        BillingPeriodTextBlock.Text = IsOneTimePayment.IsChecked ?? false ? "First payment date" : "Billing date";
    }

    private void ColorPicker_OnColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
    {
        ColorPicker.Background = new SolidColorBrush(ColorPicker.Color);
    }
}