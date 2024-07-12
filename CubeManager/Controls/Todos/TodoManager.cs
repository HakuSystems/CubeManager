using CubeManager.Controls.Todos.Enums;
using CubeManager.Controls.Todos.Models;
using CubeManager.Helpers;

namespace CubeManager.Controls.Todos;

public class TodoManager
{
    private readonly ConfigManager _configManager = ConfigManager.Instance;

    public TodoManager()
    {
        AddDefaultCategories();
    }

    private void AddDefaultCategories()
    {
        if (_configManager.Config.Todos.Categories.Count != 0) return;
        var categories = new List<TodoCategoryModel>
        {
            new() {CategoryName = TodoCategorys.All, CustomCategoryName = "All"},
            new() {CategoryName = TodoCategorys.Work, CustomCategoryName = "Work"},
            new() {CategoryName = TodoCategorys.Personal, CustomCategoryName = "Personal"},
            new() {CategoryName = TodoCategorys.School, CustomCategoryName = "School"},
            new() {CategoryName = TodoCategorys.Home,CustomCategoryName = "Home"},
            new() {CategoryName = TodoCategorys.Shopping, CustomCategoryName = "Shopping"},
            new() {CategoryName = TodoCategorys.Health, CustomCategoryName = "Health"},
            new() {CategoryName = TodoCategorys.Finance, CustomCategoryName = "Finance"},
            new() {CategoryName = TodoCategorys.Entertainment, CustomCategoryName = "Entertainment"},
            new() {CategoryName = TodoCategorys.Custom, CustomCategoryName = "Add a Custom One"}
        };
        _configManager.UpdateConfig(config => config.Todos.Categories.AddRange(categories));
    }

    public void AddTodo(Guid todoId, string name, DateTime dueDate, DateTime dueTime, TodoRepeatableType repeatableType,
        TodoStatusType todoStatus, List<TodoFilesAttachedModel> filesAttached, List<string> notes, List<string>? links,
        List<TodoCategoryModel>? category = null)
    {
        var todo = new TodoModel
        {
            TodoId = todoId,
            TodoName = name,
            DueDate = dueDate,
            DueTime = dueTime,
            TodoRepeatableType = repeatableType,
            TodoStatus = todoStatus,
            FilesAttached = filesAttached,
            Notes = notes,
            Links = links ?? new List<string>(),
            Category = category ?? new List<TodoCategoryModel>()
        };
        _configManager.UpdateConfig(config => config.Todos.Todos.Add(todo));
    }

    public void RemoveTodoById(Guid todoId)
    {
        _configManager.UpdateConfig(config =>
        {
            var index = config.Todos.Todos.FindIndex(x => x.TodoId == todoId);
            config.Todos.Todos.RemoveAt(index);
        });
    }

    public void UpdateTodoById(Guid todoId, string name, DateTime dueDate, DateTime dueTime,
        TodoRepeatableType repeatableType,
        TodoStatusType todoStatus, List<TodoFilesAttachedModel>? filesAttached, List<string>? notes, List<string>? links,
        List<TodoCategoryModel>? category = null)
    {
        var todo = new TodoModel
        {
            TodoId = todoId,
            TodoName = name,
            DueDate = dueDate,
            DueTime = dueTime,
            TodoRepeatableType = repeatableType,
            TodoStatus = todoStatus,
            FilesAttached = filesAttached ?? new List<TodoFilesAttachedModel>(),
            Notes = notes ?? new List<string>(),
            Links = links ?? new List<string>(),
            Category = category ?? new List<TodoCategoryModel>()
        };
        _configManager.UpdateConfig(config =>
        {
            var index = config.Todos.Todos.FindIndex(x => x.TodoId == todoId);
            config.Todos.Todos[index] = todo;
        });
    }

    public void AddCustomCategory(string categoryName)
    {
        var category = new TodoCategoryModel
        {
            CategoryName = TodoCategorys.Custom,
            CustomCategoryName = categoryName,
        };
        _configManager.UpdateConfig(config => config.Todos.Categories.Add(category));
    }
}