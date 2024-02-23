using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace CubeManager.Todos.Models;

public class FolderItem
{
    public Guid Id { get; set; }
    public int TotalTodos { get; set; }
    public string Name { get; set; }
    public string TotalContent { get; set; }
    public PackIconKind IconKind { get; set; }
    public bool isDefault { get; set; }
}