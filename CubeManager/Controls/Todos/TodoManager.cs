using CubeManager.Controls.Todos.Enums;
using CubeManager.Controls.Todos.Models;

namespace CubeManager.Controls.Todos;

public class TodoManager
{
    private readonly ConfigData _configData;

    public void AddTodo(Guid todoId, string Name, DateOnly DueDate, TimeOnly DueTime, TodoRepeatableType repeatableType,
        TodoStatusType todoStatus, TodoFilesAttachedModel? filesAttached, TodoNotesModel? notes, TodoLinksModel? links,
        TodoCategoryModel? category = null)
    {
        var todo = new TodoModel
        {
            TodoId = todoId == Guid.Empty ? Guid.NewGuid() : todoId,
            TodoName = Name,
            DueDate = DueDate == DateOnly.MinValue ? DateOnly.FromDateTime(DateTime.Now) : DueDate,
            DueTime = DueTime == TimeOnly.MinValue ? TimeOnly.FromDateTime(DateTime.Now) : DueTime,
            TodoRepeatableType = repeatableType == TodoRepeatableType.None ? TodoRepeatableType.None : repeatableType,
            TodoStatus = todoStatus == TodoStatusType.None ? TodoStatusType.None : todoStatus,
            FilesAttached = filesAttached == null ? new TodoFilesAttachedModel() : filesAttached,
            Notes = notes ?? new TodoNotesModel(),
            Links = links ?? new TodoLinksModel(),
            Category = category ?? new TodoCategoryModel()
        };

        _configData.Todos.Add(todo);
    }
    public void RemoveTodoById(Guid todoId)
    {
        var todo = _configData.Todos.FirstOrDefault(x => x.TodoId == todoId);
        if (todo != null)
        {
            _configData.Todos.Remove(todo);
        }
    }
    
    public void UpdateTodoById(Guid todoId, string Name, DateOnly DueDate, TimeOnly DueTime, TodoRepeatableType repeatableType,
        TodoStatusType todoStatus, TodoFilesAttachedModel? filesAttached, TodoNotesModel? notes, TodoLinksModel? links,
        TodoCategoryModel? category = null)
    {
        var todo = _configData.Todos.FirstOrDefault(x => x.TodoId == todoId);
        if (todo != null)
        {
            todo.TodoName = Name == string.Empty ? todo.TodoName : Name;
            todo.DueDate = DueDate == DateOnly.MinValue ? todo.DueDate : DueDate;
            todo.DueTime = DueTime == TimeOnly.MinValue ? todo.DueTime : DueTime;
            todo.TodoRepeatableType = repeatableType == TodoRepeatableType.None ? todo.TodoRepeatableType : repeatableType;
            todo.TodoStatus = todoStatus == TodoStatusType.None ? todo.TodoStatus : todoStatus;
            todo.FilesAttached = filesAttached == null ? todo.FilesAttached : filesAttached;
            todo.Notes = notes ?? todo.Notes;
            todo.Links = links ?? todo.Links;
            todo.Category = category ?? todo.Category;
        }
    }
    
}