using System.Windows.Documents;
using CubeManager.Controls.Todos.Models;

namespace CubeManager.Controls.Todos;

public class TodosData
{
    public List<TodoCategoryModel> Categories { get; set; } = new();
    public List<TodoModel> Todos { get; set; } = new();
    
}