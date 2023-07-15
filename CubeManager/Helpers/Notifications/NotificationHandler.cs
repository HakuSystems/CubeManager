using System.Windows.Threading;
using CubeManager.Helpers;
using CubeManager.Helpers.Notifications;
using CubeManager.Models;

namespace CubeManager.Notifications;

public class NotificationHandler
{
    private readonly Logger _logger;

    public NotificationHandler()
    {
        _logger = new Logger();
        ConfigDates = new List<NotificationData>();
        Timer = StartTimer();
    }

    private DispatcherTimer Timer { get; set; }
    private bool NotificationRunning { get; set; }
    private List<NotificationData> ConfigDates { get; set; }

    private DispatcherTimer StartTimer()
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1),
            IsEnabled = true
        };
        timer.Tick += (sender, args) => ProcessDueDates();
        timer.Start();
        return timer;
    }

    private void ProcessDueDates()
    {
        try
        {
            _logger.Info("ProcessDueDates called");
            ConfigDates = LoadAllDatesFromConfig();

            _logger.Info($"ConfigDates has {ConfigDates.Count} items");

            foreach (var date in ConfigDates)
            {
                _logger.Info(date.Date != null
                    ? $"Processing date: {date.Date}"
                    : "Encountered null date");
                // Extract Date, Hour and Minute for the comparison
                var dateCheck = new DateTime(date.Date.Year, date.Date.Month, date.Date.Day,
                    date.Date.Hour, date.Date.Minute, 0);
                var now = DateTime.Now;
                var nowCheck = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

                if (dateCheck == nowCheck)
                {
                    _logger.Info("Match found, sending notification");
                    SendNotification(date);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Error in ProcessDueDates: {ex}");
        }
    }

    private void SendNotification(NotificationData data)
    {
        try
        {
            NotificationRunning = true;
            _logger.Info($"Sending notification for {data}");
            var notification = new NotificationReminderWindow();
            notification.SetData(data);
            notification.Show();
            NotificationRunning = false;
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            throw;
        }
    }

    private List<NotificationData> LoadAllDatesFromConfig()
    {
        var config = ConfigManager.Instance.Config;

        var dates = config.Todo.Todos
            .Select(todo =>
                new NotificationData(todo.DueTime, NotificationType.Todo, todo.Description, todo.Id, todo.Title))
            .ToList();

        dates.AddRange(config.Subscriptions.Subscriptions.Select(subscription =>
            new NotificationData(subscription.NextPaymentDate, NotificationType.Subscription,
                subscription.Description, subscription.Id, subscription.Title)));

        return dates;
    }
}