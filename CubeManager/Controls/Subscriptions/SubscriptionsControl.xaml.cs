using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CubeManager.Helpers;
using CubeManager.Models.Subscriptions;
using MaterialDesignThemes.Wpf;

namespace CubeManager.Controls;

public partial class SubscriptionsControl : UserControl
{
    public SubscriptionsControl()
    {
        InitializeComponent();
    }

    private DateTime toPayForSubscription;
    private ConfigManager _configManager { get; } = new();

    private List<Subscription> Subscriptions => _configManager.Config.Subscriptions.Subscriptions;

    private string DefaultCurrency
    {
        get => _configManager.Config.Subscriptions.DefaultCurrency;
        set => _configManager.UpdateConfig(config => config.Subscriptions.DefaultCurrency = value);
    }

    private void SubscriptionsControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        CurrencyExpander.Header = $"Currency: {DefaultCurrency}";
        if (int.TryParse(BillingPeriodTextBox.Text, out var billingPeriod) && billingPeriod > 1)
            BillingExpander.Header = "Months";
        else
            BillingExpander.Header = "Month";
    }

    private void EuroButton_OnClick(object sender, RoutedEventArgs e)
    {
        DefaultCurrency = "EUR";
        CurrencyExpander.Header = $"Currency: {DefaultCurrency}";
        CurrencyExpander.IsExpanded = false;
    }

    private void UsdButton_OnClick(object sender, RoutedEventArgs e)
    {
        DefaultCurrency = "USD";
        CurrencyExpander.Header = $"Currency: {DefaultCurrency}";
        CurrencyExpander.IsExpanded = false;
    }

    private Card CreateCard(Subscription subscription)
    {
        SubscriptionScroller.Visibility = Visibility.Visible;
        EditModeScroller.Visibility = Visibility.Collapsed;
        // Create the card
        var card = new Card
        {
            Margin = new Thickness(0, 0, 0, 10),
            Padding = new Thickness(10)
        };

        // Create the grid
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Create the first stack panel
        var stackPanel1 = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        var subscriptionTitle = new Label
        {
            Content = subscription.Title,
            FontWeight = FontWeights.SemiBold
        };


        if (Convert.ToInt32(subscription.Cost) % 1 == 0) subscription.Cost = Convert.ToInt32(subscription.Cost) / 100;

        var subscriptionPrice = new TextBlock
        {
            Text = subscription.Cost % 1 == 0
                ? $"{Convert.ToInt32(subscription.Cost)} {subscription.Currency}"
                : $"{Convert.ToDouble(subscription.Cost)} {subscription.Currency}", // If the cost is an integer, don't show the decimal part
            HorizontalAlignment = HorizontalAlignment.Left,
            FontSize = 24,
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Center
        };

        var subscriptionDescription = new TextBlock
        {
            Text = subscription.Description ?? "No description",
            FontStyle = FontStyles.Italic,
            FontSize = 12,
            Opacity = 0.7,
            TextWrapping = TextWrapping.Wrap
        };

        stackPanel1.Children.Add(subscriptionTitle);
        stackPanel1.Children.Add(subscriptionPrice);
        stackPanel1.Children.Add(subscriptionDescription);

        Grid.SetColumn(stackPanel1, 0);
        grid.Children.Add(stackPanel1);

        // Create the second stack panel
        var stackPanel2 = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center
        };

        // Accessing dynamic resource
        var separatorBrush = Application.Current.Resources["MaterialDesignDarkSeparatorBackground"] as Brush;

        var innerCard = new Card
        {
            Padding = new Thickness(10),
            Background =
                separatorBrush ?? new SolidColorBrush(Colors.Gray) // Fallback to gray if the resource is not found
        };

        var innerStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        var subscriptionEditButton = new Button();
        var icon1 = new PackIcon { Kind = PackIconKind.EditOutline };
        subscriptionEditButton.Content = icon1;

        // Accessing style resource
        var iconButtonStyle = Application.Current.Resources["MaterialDesignIconButton"] as Style;

        // Applying style to the button
        if (iconButtonStyle != null) subscriptionEditButton.Style = iconButtonStyle;


        var innerTextStackPanel = new StackPanel { Orientation = Orientation.Vertical };


        var timeLeft = CalcDateTimeLeft(subscription.FirstPaymentDate, subscription.BillingPeriod,
            subscription.IsOneTimePayment, subscription.BillingPeriodType);

        var subscriptionLeftShortTime = new TextBlock
        {
            Text = timeLeft,
            HorizontalAlignment = HorizontalAlignment.Center,
            FontSize = 24,
            FontWeight = FontWeights.Bold
        };

        var subscriptionLeftText = new TextBlock
        {
            Text = "LEFT",
            HorizontalAlignment = HorizontalAlignment.Center,
            FontSize = 15,
            FontWeight = FontWeights.SemiBold
        };

        innerTextStackPanel.Children.Add(subscriptionLeftShortTime);
        innerTextStackPanel.Children.Add(subscriptionLeftText);

        var subscriptionDeleteButton = new Button();
        var icon2 = new PackIcon { Kind = PackIconKind.DeleteOutline };
        subscriptionDeleteButton.Content = icon2;

        if (iconButtonStyle != null) subscriptionDeleteButton.Style = iconButtonStyle;

        innerStackPanel.Children.Add(subscriptionEditButton);
        innerStackPanel.Children.Add(innerTextStackPanel);
        innerStackPanel.Children.Add(subscriptionDeleteButton);

        innerCard.Content = innerStackPanel;
        stackPanel2.Children.Add(innerCard);

        Grid.SetColumn(stackPanel2, 1);
        grid.Children.Add(stackPanel2);

        // Add the grid to the card
        card.Content = grid;

        return card;
    }

    private string CalcDateTimeLeft(DateTime firstPaymentDate, int period, bool oneTimePayment, string periodType)
    {
        //EXAMPLE
        // firstPaymentDate = 01.01.2023
        // period = 2
        // periodType = "Month" or "Months" alwell as "Year" or "Years"  and "Day" or "Days" and "Week" or "Weeks"
        // oneTimePayment = false


        var nextPaymentDate = GetPeriodDate(firstPaymentDate, period, periodType);

        var difference = firstPaymentDate - nextPaymentDate;

        var days = Math.Abs(difference.Days);
        if (oneTimePayment)
        {
            var oneTimeDif = DateTime.Now - firstPaymentDate;
            var oneTimeDays = Math.Abs(oneTimeDif.Days);
            switch (oneTimeDays)
            {
                case 0:
                    return "Today";
                case var d when d < 30:
                    return $"{d}D";
                case var w when w < 365:
                    return $"{w / 7}W";
                case var m when m < 365:
                    return $"{m / 30}M";
                default:
                    return $"{oneTimeDays / 365}Y";
            }
        }

        switch (days)
        {
            case 0:
                return "Today";
            case var d when d < 30:
                return $"{d}D";
            case var w when w < 365:
                return $"{w / 7}W";
            case var m when m < 365:
                return $"{m / 30}M";
            default:
                return $"{days / 365}Y";
        }
        toPayForSubscription = subscription;
    }

    private DateTime GetPeriodDate(DateTime firstPaymentDate, int period, string periodType)
    {
        var nextPaymentDate = firstPaymentDate;
        switch (periodType)
        {
            case "Day":
            case "Days":
                nextPaymentDate = firstPaymentDate.AddDays(period);
                break;
            case "Week":
            case "Weeks":
                nextPaymentDate = firstPaymentDate.AddDays(period * 7);
                break;
            case "Month":
            case "Months":
                nextPaymentDate = firstPaymentDate.AddMonths(period);
                break;
            case "Year":
            case "Years":
                nextPaymentDate = firstPaymentDate.AddYears(period);
                break;
        }

        return nextPaymentDate;
    }

    private void AddSubscriptionButton_OnClick(object sender, RoutedEventArgs e)
    {
        SubscriptionScroller.Visibility = Visibility.Collapsed;
        EditModeScroller.Visibility = Visibility.Visible;
        CurrencyExpander.Visibility = Visibility.Visible;
    }

    private void FirstPaymentBtn_OnClick(object sender, RoutedEventArgs e)
    {
        DialogHostOperation.IsOpen = true;
    }

    private void SelectDateBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedDate = PaymentDateCalendar.SelectedDate;
        if (selectedDate.HasValue)
            FirstPaymentSetDate.Text = $"Set to: {selectedDate.Value.ToShortDateString()}";
        else
            FirstPaymentSetDate.Text = "Set to: None";

        DialogHostOperation.IsOpen = false;
    }

    private void OneTimePaymentBtn_OnClick(object sender, RoutedEventArgs e)
    {
        OneTimeCheck.IsChecked = !OneTimeCheck.IsChecked;
    }

    private void OneTimeCheck_OnUnchecked(object sender, RoutedEventArgs e)
    {
        BillingExpander.Visibility = Visibility.Visible;
        BillingPeriodTextBox.Visibility = Visibility.Visible;
        BilledEveryTextBlock.Visibility = Visibility.Visible;
        OneTimePaymentBtnText.Foreground = Brushes.White;
        FirstPaymentBtn.Content = "Select First Payment Date";
        FirstPaymentSetDate.Text = "This is Optional";
    }

    private void OneTimeCheck_OnChecked(object sender, RoutedEventArgs e)
    {
        BillingExpander.Visibility = Visibility.Collapsed;
        BillingPeriodTextBox.Visibility = Visibility.Collapsed;
        BilledEveryTextBlock.Visibility = Visibility.Collapsed;
        OneTimePaymentBtnText.Foreground = Brushes.Crimson;
        FirstPaymentBtn.Content = "Select Payment Date";
    }

    private void UpdateBillingHeader(string singular, string plural)
    {
        if (int.TryParse(BillingPeriodTextBox.Text, out var billingPeriod) && billingPeriod > 1)
            BillingExpander.Header = plural;
        else
            BillingExpander.Header = singular;
        BillingExpander.IsExpanded = false;
    }

    private void UpdateButtonText(Button button, string singular, string plural)
    {
        if (int.TryParse(BillingPeriodTextBox.Text, out var billingPeriod) && billingPeriod > 1)
            ((TextBlock)((StackPanel)button.Content).Children[1]).Text = plural;
        else
            ((TextBlock)((StackPanel)button.Content).Children[1]).Text = singular;
    }

    private void UpdateBillingHeaderAndButtonText(string singular, string plural)
    {
        UpdateBillingHeader(singular, plural);
        UpdateButtonText(DayBillingBtn, "Day", "Days");
        UpdateButtonText(WeekBillingBtn, "Week", "Weeks");
        UpdateButtonText(MonthBillingBtn, "Month", "Months");
        UpdateButtonText(YearBillingBtn, "Year", "Years");
    }

    private void DayBillingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateBillingHeaderAndButtonText("Day", "Days");
    }

    private void MonthBillingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateBillingHeaderAndButtonText("Month", "Months");
    }

    private void WeekBillingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateBillingHeaderAndButtonText("Week", "Weeks");
    }

    private void YearBillingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateBillingHeaderAndButtonText("Year", "Years");
    }

    private void BillingPeriodTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!int.TryParse(BillingPeriodTextBox.Text, out var billingPeriod) || billingPeriod < 1)
            BillingPeriodTextBox.Text = "1";

        var header = BillingExpander.Header.ToString();
        var singular = header.EndsWith("s") ? header.Substring(0, header.Length - 1) : header;
        var plural = singular + "s";
        UpdateBillingHeaderAndButtonText(singular, plural);
    }

    private void SaveSubscriptionBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var title = SubscriptionNameTextBox.Text;
        if (string.IsNullOrEmpty(title))
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue("Title is required.");
            return;
        }

        if (string.IsNullOrEmpty(SubscriptionCostTextBox.Text) ||
            !decimal.TryParse(SubscriptionCostTextBox.Text, out var cost))
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue("Price is required.");
            return;
        }

        var currency = DefaultCurrency;
        if (string.IsNullOrEmpty(currency))
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue("Currency is required.");
            return;
        }

        var description = SubscriptionDescriptionTextBox.Text;

        var isOneTimePayment = OneTimeCheck.IsChecked!.Value;


        if (string.IsNullOrEmpty(BillingPeriodTextBox.Text) && isOneTimePayment) //monthly = 0
            BillingPeriodTextBox.Text = 1.ToString();


        var billingPeriodType = BillingExpander.Header.ToString();
        if (isOneTimePayment) billingPeriodType = "One Time Payment";

        var firstPaymentDate = Convert.ToDateTime(FirstPaymentSetDate.Text.Contains("Set to:")
            ? PaymentDateCalendar.SelectedDate!.Value.ToShortDateString()
            : DateTime.Now.ToShortDateString());
        if (string.IsNullOrEmpty(firstPaymentDate.ToString()) && isOneTimePayment)
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue(
                "Payment date is required.");
            return;
        }


        var subscription = new Subscription
        {
            Title = title,
            Cost = (int)cost,
            Currency = currency,
            Description = description,
            BillingPeriod = Convert.ToInt32(BillingPeriodTextBox.Text),
            BillingPeriodType = billingPeriodType,
            IsOneTimePayment = isOneTimePayment,
            FirstPaymentDate = firstPaymentDate,
            ToPayPaymentDate = toPayForSubscription
        };
        SubscriptionScroller.Visibility = Visibility.Visible;
        EditModeScroller.Visibility = Visibility.Collapsed;
        CurrencyExpander.Visibility = Visibility.Collapsed;

        Subscriptions.Add(subscription);
        _configManager.UpdateConfig(config => config.Subscriptions.Subscriptions = Subscriptions);

        var card = CreateCard(subscription);
        SubscriptionsStackPanel.Children.Add(card);
    }

    private void SubscriptionPriceTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!double.TryParse(SubscriptionCostTextBox.Text, out var subscriptionCost) || subscriptionCost < 0)
            SubscriptionCostTextBox.Text = "0.00";
    }

    private void DebugBtn_OnClick(object sender, RoutedEventArgs e)
    {
        Subscriptions.Clear();
        _configManager.UpdateConfig(config => config.Subscriptions.Subscriptions = Subscriptions);
    }
}