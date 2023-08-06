using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace CubeManager.Todos.Models;

public class FolderItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public PackIconKind IconKind { get; set; }
    public bool isDefault { get; set; }
}