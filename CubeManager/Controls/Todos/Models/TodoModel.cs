using CubeManager.Controls.Todos.Enums;

namespace CubeManager.Controls.Todos.Models;

public class TodoModel
{
    public Guid TodoId { get; set; } // unique identifier
    public string TodoName { get; set; }
    public DateOnly DueDate { get; set; }
    public TimeOnly DueTime { get; set; }
    public TodoRepeatableType TodoRepeatableType { get; set; } // daily, weekly, monthly, yearly
    public TodoStatusType TodoStatus { get; set; } // none, completed
    public TodoFilesAttachedModel? FilesAttached { get; set; } // optional
    public TodoNotesModel? Notes { get; set; } // optional
    public TodoLinksModel? Links { get; set; } // optional
    public TodoCategoryModel? Category { get; set; } // optional
}