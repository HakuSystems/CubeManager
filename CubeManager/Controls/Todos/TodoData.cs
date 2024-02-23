using CubeManager.Todos.Models;

namespace CubeManager.Todos;

public class TodoData
{
    public List<TodoItem> Todos { get; set; } = new();
    public List<TodoSettings> Settings { get; set; } = new();
    public List<FolderItem> Folders { get; set; } = new();
}
