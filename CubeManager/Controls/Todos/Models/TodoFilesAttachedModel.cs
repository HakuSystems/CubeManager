namespace CubeManager.Controls.Todos.Models;

public class TodoFilesAttachedModel
{
    public string FileName { get; set; }
    public string FilePath { get; set; } // relative path
    public string FileExtension { get; set; }
    public string FileSize { get; set; } // in bytes
}