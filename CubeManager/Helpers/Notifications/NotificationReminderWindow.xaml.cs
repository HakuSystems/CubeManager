using System.Windows;
using CubeManager.Models;
using CubeManager.Notifications;

namespace CubeManager.Helpers.Notifications;

public partial class NotificationReminderWindow : Window
{
    private readonly SoundManager _soundManager = new();

    public NotificationReminderWindow()
    {
        InitializeComponent();
    }

    private void BtnDismiss_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public void SetData(NotificationData data)
    {
        NotificationTypeText.Text = data.Type switch
        {
            NotificationType.Todo => $"Todo {data.Title}",
            NotificationType.Subscription => $"Subscription {data.Title}",
            _ => NotificationTypeText.Text
        };

        NotificationDescriptionText.Text = data.Description;
        NotificationIDText.Text = data.Id.ToString();
    }

    private void NotificationReminderWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.TaskComplete);
    }
}