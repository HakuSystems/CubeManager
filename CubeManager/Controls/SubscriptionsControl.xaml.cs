using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CubeManager.Helpers;
using CubeManager.Models.Subscriptions;
using MaterialDesignThemes.Wpf;

namespace CubeManager.Controls;

public partial class SubscriptionsControl : UserControl
{
    private readonly Logger _logger;
    private Color _colorForCard;
    private DateTime _toPayDate;


    public SubscriptionsControl()
    {
        _logger = new Logger();
        InitializeComponent();
    }

    private List<Subscription> Subscriptions => ConfigManager.Instance.Config.Subscriptions.Subscriptions;

    private void SubscriptionsControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        _logger.Info("SubscriptionsControl loaded");
        UpdateUi();
    }

    private void UpdateUi()
    {
        _logger.Info("Updating UI");

        EditableCard.Background = CreateGradientBrush(ColorPickerOnDialog.Color);
        if (Subscriptions.Count == 0)
        {
            _logger.Info("No subscriptions found, switching to edit mode");

            SwitchToEditMode();
            return;
        }

        //Subscriptions.Sort by NextPaymentDate
        Subscriptions.Sort((subscription1, subscription2) =>
            subscription1.NextPaymentDate.CompareTo(subscription2.NextPaymentDate));

        _logger.Info("Subscriptions sorted");

        //show sorted subscriptions with CreateCard
        foreach (var subscription in Subscriptions)
            SubscriptionsStackPanel.Children.Add(CreateCard(subscription));
    }

    private void SwitchToEditMode()
    {
        SubscriptionScroller.Visibility = Visibility.Collapsed;
        EditModeScroller.Visibility = Visibility.Visible;

        _logger.Info("Switched to edit mode");
    }

    private void UpdateBillingHeader(Expander billingExpander, string singular, string plural)
    {
        //todo: fix this
    }

    private void EuroButton_OnClick(object sender, RoutedEventArgs e)
    {
        CurrencyExpander.Header = "Currency: EUR";
        CurrencyExpander.IsExpanded = false;

        _logger.Info("Currency changed to EUR");
    }

    private void UsdButton_OnClick(object sender, RoutedEventArgs e)
    {
        CurrencyExpander.Header = "Currency: USD";
        CurrencyExpander.IsExpanded = false;

        _logger.Info("Currency changed to USD");
    }

    private Card CreateCard(Subscription subscription)
    {
        _logger.Info($"Creating card for subscription {subscription.Title}");

        SubscriptionScroller.Visibility = Visibility.Visible;
        EditModeScroller.Visibility = Visibility.Collapsed;
        // Create the card
        var card = new Card
        {
            Margin = new Thickness(0, 0, 0, 10),
            Padding = new Thickness(10),
            Background = CreateGradientBrush(subscription.CardColor)
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


        var subscriptionPrice = new TextBlock
        {
            Text = $"{subscription.Cost} {subscription.Currency}",
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
        var editIcon = new PackIcon { Kind = PackIconKind.EditOutline };
        subscriptionEditButton.Content = editIcon;

        // Accessing style resource
        var iconButtonStyle = Application.Current.Resources["MaterialDesignIconButton"] as Style;

        // Applying style to the button
        if (iconButtonStyle != null) subscriptionEditButton.Style = iconButtonStyle;


        var innerTextStackPanel = new StackPanel { Orientation = Orientation.Vertical };


        var timeLeft = CalcDateTimeLeft(subscription.FirstPaymentDate, subscription.Period, subscription.PeriodType);

        _logger.Info($"Time left for subscription {subscription.Title} is {timeLeft}");

        var subscriptionLeftShortTime = new TextBlock
        {
            Text = timeLeft,
            ToolTip = $"Time left until the next payment: {subscription.NextPaymentDate.ToShortDateString()}",
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
        var deleteIcon = new PackIcon { Kind = PackIconKind.DeleteOutline };
        subscriptionDeleteButton.Content = deleteIcon;

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

        subscriptionEditButton.Click += SubscriptionEditBtn_Click(subscription.Id, card);
        subscriptionDeleteButton.Click += SubscriptionDeleteBtn_Click(subscription.Id, subscription.Title, card);
        return card;
    }


    private Brush CreateGradientBrush(Color subscriptionCardColor)
    {
        var brush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1)
        };

        var gradientStop1 = new GradientStop
        {
            Color = subscriptionCardColor,
            Offset = 0
        };

        var color = new MaterialDesignDarkTheme();
        var gradientStop2 = new GradientStop
        {
            Color = color.MaterialDesignCardBackground,
            Offset = 1
        };

        brush.GradientStops.Add(gradientStop1);
        brush.GradientStops.Add(gradientStop2);

        return brush;
    }

    private RoutedEventHandler SubscriptionEditBtn_Click(Guid subscriptionId, Card card)
    {
        return (sender, args) =>
        {
            Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);
            var subscription = Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);
            if (subscription != null)
            {
                SubscriptionNameTextBox.Text = subscription.Title;
                SubscriptionCostTextBox.Text = subscription.Cost;
                SubscriptionDescriptionTextBox.Text = subscription.Description;
                BillingPeriodTextBox.Text = subscription.Period.ToString();
                PaymentDateCalendarOnDialog.SelectedDate = subscription.FirstPaymentDate;
                BillingExpander.Header = subscription.PeriodType;
                Subscriptions.Remove(subscription);
                ConfigManager.Instance.UpdateConfig(config => config.Subscriptions.Subscriptions = Subscriptions);
                SubscriptionsStackPanel.Children.Remove(card);
                SwitchToEditMode();
            }
        };
    }

    private RoutedEventHandler SubscriptionDeleteBtn_Click(Guid subscriptionId, string? subscriptionTitle, Card card)
    {
        return (sender, args) =>
        {
            var result = MessageBox.Show($"Are you sure you want to delete {subscriptionTitle}?", "Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            var subscription = Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);
            if (subscription != null)
            {
                Subscriptions.Remove(subscription);
                ConfigManager.Instance.UpdateConfig(config => config.Subscriptions.Subscriptions = Subscriptions);
                SubscriptionsStackPanel.Children.Remove(card);
            }
        };
    }

    private string CalcDateTimeLeft(DateTime firstPaymentDate, int period, string periodType)
    {
        _logger.Info($"Calculating time left for subscription {firstPaymentDate} {period} {periodType}");

        var nextPaymentDate = GetPeriodDate(firstPaymentDate, period, periodType);
        var daysLeft = (GetPeriodDate(firstPaymentDate, period, periodType) - DateTime.Now).Days + 1;
        var weeksLeft = (GetPeriodDate(firstPaymentDate, period, periodType) - DateTime.Now).Days / 7;
        var monthsLeft = (GetPeriodDate(firstPaymentDate, period, periodType) - DateTime.Now).Days / 30;
        var yearsLeft = (GetPeriodDate(firstPaymentDate, period, periodType) - DateTime.Now).Days / 365;


        var difference = GetPeriodDate(firstPaymentDate, period, periodType) - DateTime.Now;
        var timeLeft = "";
        if (difference.Days < 0)
            timeLeft = "0D";
        else
            switch (periodType)
            {
                case "Day":
                case "Days":
                    timeLeft = $"{daysLeft}D";
                    break;
                case "Week":
                case "Weeks":
                    timeLeft = $"{weeksLeft}W";
                    break;
                case "Month":
                case "Months":
                    timeLeft = $"{monthsLeft}M";
                    break;
                case "Year":
                case "Years":
                    timeLeft = $"{yearsLeft}Y";
                    break;
            }

        _toPayDate = nextPaymentDate;

        _logger.Info($"Time left for subscription {firstPaymentDate} {period} {periodType} is {timeLeft}");

        return timeLeft;
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
        _logger.Info("Add subscription button clicked");
    }

    private void SelectDateBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedDate = PaymentDateCalendarOnDialog.SelectedDate;
        if (!selectedDate.HasValue)
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue("Please Choose a date");
            _logger.Critical("No date selected");
            return;
        }

        _logger.Info($"Selected date is {selectedDate}");
        DialogHostOperation.IsOpen = false;
    }

    private void UpdateButtonText(Button button, string singular, string plural)
    {
        if (int.TryParse(BillingPeriodTextBox.Text, out var billingPeriod) && billingPeriod > 1)
            ((TextBlock)((StackPanel)button.Content).Children[1]).Text = plural;
        else
            ((TextBlock)((StackPanel)button.Content).Children[1]).Text = singular;

        _logger.Info($"Button text for {button.Name} updated");
    }

    private void UpdateBillingHeaderAndButtonText(string singular, string plural)
    {
        UpdateButtonText(DayBillingBtn, singular, plural);
        UpdateButtonText(WeekBillingBtn, singular, plural);
        UpdateButtonText(MonthBillingBtn, singular, plural);
        UpdateButtonText(YearBillingBtn, singular, plural);
        //UpdateBillingHeader(BillingExpander,singular, plural);
        _logger.Info($"Billing header and button text updated");
    }

    private void DayBillingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateBillingHeaderAndButtonText("Day", "Days");
        BillingExpander.IsExpanded = false;
    }

    private void MonthBillingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateBillingHeaderAndButtonText("Month", "Months");
        BillingExpander.IsExpanded = false;
    }

    private void WeekBillingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateBillingHeaderAndButtonText("Week", "Weeks");
        BillingExpander.IsExpanded = false;
    }

    private void YearBillingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateBillingHeaderAndButtonText("Year", "Years");
        BillingExpander.IsExpanded = false;
    }

    private void BillingPeriodTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!int.TryParse(BillingPeriodTextBox.Text, out var billingPeriod) || billingPeriod < 1)
            BillingPeriodTextBox.Text = "1";

        _logger.Info($"Billing period text changed to {BillingPeriodTextBox.Text}");

        var header = BillingExpander.Header.ToString();
        var singular = header.EndsWith("s") ? header.Substring(0, header.Length - 1) : header;
        var plural = singular + "s";

        _logger.Info($"Singular is {singular} and plural is {plural}");

        UpdateBillingHeaderAndButtonText(singular, plural);
    }

    private void SaveSubscriptionBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (!ValidateInputs())
            return;

        _logger.Info("Inputs validated");


        #region Checking for Compilations

        var billingPeriodType = BillingExpander.Header.ToString();


        var firstPaymentDate = Convert.ToDateTime(PaymentDateCalendarOnDialog.SelectedDate.Value.ToShortDateString());


        CalcDateTimeLeft(firstPaymentDate, Convert.ToInt32(BillingPeriodTextBox.Text),
            billingPeriodType);

        #endregion

        var subscription = CreateSubscription();
        AddSubscription(subscription);
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrEmpty(SubscriptionNameTextBox.Text))
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue("Title is required.");
            return false;
        }

        if (string.IsNullOrEmpty(SubscriptionCostTextBox.Text) ||
            !decimal.TryParse(SubscriptionCostTextBox.Text, out var cost))
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue("Price is required.");
            return false;
        }

        if (CurrencyExpander.Header.Equals("Currency:") || string.IsNullOrEmpty(CurrencyExpander.Header.ToString()))
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue("Currency is required.");
            return false;
        }

        if (string.IsNullOrEmpty(PaymentDateCalendarOnDialog.SelectedDate?.ToShortDateString()))
        {
            ErrorHandlingSnackbar.MessageQueue?.Enqueue("Payment date is required.");
            return false;
        }

        if (string.IsNullOrEmpty(BillingPeriodTextBox.Text))
            BillingPeriodTextBox.Text = "1";

        return true;
    }

    private Subscription CreateSubscription()
    {
        var title = SubscriptionNameTextBox.Text;
        var description = SubscriptionDescriptionTextBox.Text;
        var billingPeriodType = BillingExpander.Header.ToString();
        var firstPaymentDate = Convert.ToDateTime(PaymentDateCalendarOnDialog.SelectedDate.Value.ToShortDateString());

        CalcDateTimeLeft(firstPaymentDate, Convert.ToInt32(BillingPeriodTextBox.Text), billingPeriodType);

        _logger.Info($"Subscription created with title {title} and description {description}");

        return new Subscription
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Cost = SubscriptionCostTextBox.Text,
            Currency = CurrencyExpander.Header.ToString()?.Substring(10),
            Period = Convert.ToInt32(BillingPeriodTextBox.Text),
            PeriodType = billingPeriodType,
            FirstPaymentDate = firstPaymentDate,
            NextPaymentDate = _toPayDate,
            CardColor = _colorForCard
        };
    }

    private void AddSubscription(Subscription subscription)
    {
        Subscriptions.Add(subscription);
        ConfigManager.Instance.UpdateConfig(config => config.Subscriptions.Subscriptions = Subscriptions);

        var card = CreateCard(subscription);
        SubscriptionsStackPanel.Children.Add(card);

        SwitchToSubscriptionView();
    }

    private void SwitchToSubscriptionView()
    {
        SubscriptionScroller.Visibility = Visibility.Visible;
        EditModeScroller.Visibility = Visibility.Collapsed;
        CurrencyExpander.Visibility = Visibility.Collapsed;

        _logger.Info("Switched to subscription view");
    }

    private void SubscriptionPriceTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(SubscriptionCostTextBox.Text)) return;
        if (decimal.TryParse(SubscriptionCostTextBox.Text, out var cost))
            SubscriptionCostTextBox.Text = cost.ToString("0.00");
        else
            SubscriptionCostTextBox.Text = "0.00";

        _logger.Info($"Subscription price text changed to {SubscriptionCostTextBox.Text}");
    }

    private void OpenCalenderBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (SelectDateBtnOnDialog.Visibility == Visibility.Collapsed)
            SelectDateBtnOnDialog.Visibility = Visibility.Visible;

        if (PaymentDateCalendarOnDialog.Visibility == Visibility.Collapsed)
            PaymentDateCalendarOnDialog.Visibility = Visibility.Visible;

        _logger.Info("Opening calendar");

        DialogHostOperation.IsOpen = true;
    }

    private void ChangeBackgroundBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (ColorPickerOnDialog.Visibility == Visibility.Collapsed)
            ColorPickerOnDialog.Visibility = Visibility.Visible;

        if (SelectColorBtnOnDialog.Visibility == Visibility.Collapsed)
            SelectColorBtnOnDialog.Visibility = Visibility.Visible;

        _logger.Info("Opening color picker");

        DialogHostOperation.IsOpen = true;
    }

    private void SelectColorBtn_OnClick(object sender, RoutedEventArgs e)
    {
        EditableCard.Background = CreateGradientBrush(ColorPickerOnDialog.Color);
        _colorForCard = ColorPickerOnDialog.Color;

        _logger.Info($"Color for card is {_colorForCard}");

        DialogHostOperation.IsOpen = false;
    }


    private void DialogHostOperation_OnDialogClosed(object sender, DialogClosedEventArgs eventargs)
    {
        SelectDateBtnOnDialog.Visibility = Visibility.Collapsed;
        PaymentDateCalendarOnDialog.Visibility = Visibility.Collapsed;
        ColorPickerOnDialog.Visibility = Visibility.Collapsed;
        SelectColorBtnOnDialog.Visibility = Visibility.Collapsed;

        _logger.Info("Dialog closed");
    }
}