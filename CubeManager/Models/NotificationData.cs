using CubeManager.Notifications;

namespace CubeManager.Models;

public class NotificationData
{
    public NotificationData(DateTime date, NotificationType type, string description, Guid id, string title)
    {
        Date = date;
        Type = type;
        Description = description;
        Id = id;
        Title = title;
    }

    public string Title { get; set; }
    public DateTime Date { get; set; }
    public NotificationType Type { get; set; }
    public string Description { get; set; }
    public Guid Id { get; set; }
}