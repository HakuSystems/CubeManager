namespace CubeManager.Todos.Models;

public class TodoItem
{
    public Guid Id { get; set; } // 12345678-1234-1234-1234-123456789012
    public string Title { get; set; } // Touch grass
    public string Description { get; set; } // you have to Touch grass
    public DateTime DueDate { get; set; } // 2021-08-01
    public DateTime DueTime { get; set; } // 12:00
    
    public string Repeat { get; set; } // daily, weekly, monthly, yearly
    
    public bool MarkedAsImportant { get; set; }
    
    public bool MarkedAsCompleted { get; set; } 
    
    public FolderItem InFolder { get; set; }
    
    public string Notes { get; set; }
    
    public List<LinkItem> Links { get; set; }
    
}