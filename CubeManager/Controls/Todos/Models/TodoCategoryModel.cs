namespace CubeManager.Controls.Todos.Models;

public class TodoCategoryModel
{
    public Guid CategoryId { get; set; } = Guid.NewGuid();  // unique identifier
    public string CategoryName { get; set; } = "Default";
    public string? CategoryDescription { get; set; } // optional

}