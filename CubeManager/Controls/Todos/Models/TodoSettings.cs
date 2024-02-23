namespace CubeManager.Todos.Models;

public class TodoSettings
{
    public int TotalFolders { get; set; }
    
    public int TotalTodos { get; set; }
    
    public int TotalImportantTodos { get; set; }
    
    public int TotalPlannedTodos { get; set; }
    
    public int TotalCompletedTodos { get; set; }
    public Guid SelectedFolder { get; set; }
}