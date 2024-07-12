using System;
using CubeManager.Controls.Todos.Enums;

namespace CubeManager.Controls.Todos.Models
{
    public class TodoCategoryModel
    {
        public TodoCategorys CategoryName { get; set; }
        public string? CustomCategoryName { get; set; } // optional
    }
}