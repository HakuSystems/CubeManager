using System;
using System.Collections.Generic;
using CubeManager.Controls.Todos.Enums;

namespace CubeManager.Controls.Todos.Models
{
    public class TodoModel
    {
        public Guid TodoId { get; set; } // unique identifier
        public string TodoName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DueTime { get; set; }
        public TodoRepeatableType TodoRepeatableType { get; set; } // daily, weekly, monthly, yearly
        public TodoStatusType TodoStatus { get; set; } // none, completed
        public List<TodoFilesAttachedModel> FilesAttached { get; set; } // optional
        public List<TodoCategoryModel> Category { get; set; } // optional
        public string Notes { get; set; } // optional
        public List<string> Links { get; set; } // optional
    }
}