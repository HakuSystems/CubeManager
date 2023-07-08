namespace CubeManager.Models;

public class TodoData
{
    public List<TodoItem> Todos { get; set; } = new();
}

public class TodoItem
{
    public Guid Id { get; set; } // 12345678-1234-1234-1234-123456789012
    public string Title { get; set; } // Cut the grass
    public string Description { get; set; } // you have to cut the grass
    public DateTime DueDate { get; set; } // 2021-08-01
    public DateTime DueTime { get; set; } // 12:00
}