using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CubeManager.Controls.Todos.Enums;
using CubeManager.Controls.Todos.Models;
using CubeManager.CustomMessageBox;
using CubeManager.Helpers;

namespace CubeManager.Controls.Todos;

public partial class TodoControl : UserControl
{
    private readonly ConfigManager _configManager = ConfigManager.Instance;
    private readonly TodoManager _todoManager = new();
    private readonly List<TodoFilesAttachedModel> _filesAttached = new();
    private readonly List<string> _linksAttached = new();
    private readonly Logger _logger = new();

    public TodoControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void TodoControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        _logger.Info("TodoControl Loaded");
        if (_configManager.Config.Todos.Todos.Count == 0)
            OnLoadedNewTodo();
        else
            OnLoadedExistingTodo();
    }

    private void OnLoadedExistingTodo()
    {
        TodoViewSection.Visibility = Visibility.Visible;
        //E todo
    }

    private void OnLoadedNewTodo()
    {
        TodoCreationSection.Visibility = Visibility.Visible;
        ExampleName.Text = TodoNameTxtBox.Text ?? "Example Todo";
        ExampleDueDate.Text = DueDatePicker.SelectedDate?.ToString("dd/MM/yyyy");
        ExampleDueTime.Text = DueTimePicker.SelectedTime?.ToString("HH:mm");
        ExampleFrequency.Text = ((TodoRepeatableType)FrequencyComboBox.SelectedIndex).ToString();
        ExampleNote.Text = NotesTxtBox.Text ?? "Example Note";
        ExampleCategory.Text = CategoryComboBox.SelectedIndex == 0
            ? "All"
            : CategoryComboBox.SelectedItem?.ToString() ?? "";
        if (_filesAttached.Count > 0)
        {
            ExampleFilesCounter.Content = _filesAttached.Count.ToString();
            ExampleViewFilesBtn.Visibility = Visibility.Visible;
        }
        else
        {
            ExampleViewFilesBtn.Visibility = Visibility.Collapsed;
        }

        if (_linksAttached.Count > 0)
        {
            ExampleLinksCounter.Content = _linksAttached.Count.ToString();
            ExampleViewLinksBtn.Visibility = Visibility.Visible;
        }
        else
        {
            ExampleViewLinksBtn.Visibility = Visibility.Collapsed;
        }
    }

    private void TodoNameTxtBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ExampleName.Text = TodoNameTxtBox.Text;
    }

    private void DueDatePicker_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        ExampleDueDate.Text = DueDatePicker.SelectedDate?.ToString("dd/MM/yyyy");
    }

    private void DueTimePicker_OnSelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        ExampleDueTime.Text = DueTimePicker.SelectedTime?.ToString("HH:mm");
    }

    private void FrequencyComboBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        FrequencyComboBox.ItemsSource = Enum.GetValues(typeof(TodoRepeatableType));
    }

    private void FrequencyComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ExampleFrequency.Text = ((TodoRepeatableType)FrequencyComboBox.SelectedIndex).ToString();
    }

    private void NotesTxtBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ExampleNote.Text = NotesTxtBox.Text;
    }

    private void CategoryComboBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        CategoryComboBox.ItemsSource = _configManager.Config.Todos.Categories.Select(x => x.CustomCategoryName).ToList();
    }

    private void CategoryComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ExampleCategory.Text = CategoryComboBox.SelectedIndex == 0
            ? "All"
            : CategoryComboBox.SelectedItem?.ToString() ?? "";

        CustomCategoryTxtBox.Visibility =
            CategoryComboBox.SelectedIndex == 9 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void CustomCategoryTxtBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ExampleCategory.Text = CustomCategoryTxtBox.Text;
    }

    private void AddLinkBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var link = LinkTxtBox.Text;
        if (!string.IsNullOrWhiteSpace(link))
        {
            _linksAttached.Add(link);
            ExampleLinksCounter.Content = _linksAttached.Count.ToString();
            ExampleViewLinksBtn.Visibility = Visibility.Visible;
        }

        LinkTxtBox.Clear();
    }

    private void AttachFilesCard_OnDragEnter(object sender, DragEventArgs e)
    {
        AttachFilesCard.BorderBrush = new SolidColorBrush(Colors.Aqua);
    }

    private void AttachFilesCard_OnDrop(object sender, DragEventArgs e)
    {
        AttachFilesCard.BorderBrush = new SolidColorBrush(Colors.Transparent);
        if (e.Data.GetData(DataFormats.FileDrop) is string[] fileNames)
            foreach (var fileName in fileNames)
                if (File.Exists(fileName))
                {
                    var FilePath = fileName;
                    var fileExtension = Path.GetExtension(fileName);
                    var fileSize = new FileInfo(fileName).Length;
                    var file = new TodoFilesAttachedModel
                    {
                        FileName = Path.GetFileName(fileName),
                        FilePath = FilePath,
                        FileExtension = fileExtension,
                        FileSize = fileSize.ToString()
                    };
                    _filesAttached.Add(file);
                }

        ExampleFilesCounter.Content = _filesAttached.Count.ToString();
        ExampleViewFilesBtn.Visibility = Visibility.Visible;
    }

    private void AttachFilesCard_OnDragLeave(object sender, DragEventArgs e)
    {
        AttachFilesCard.BorderBrush = new SolidColorBrush(Colors.Transparent);
    }

    private void AddNewToDoBtn_OnClick(object sender, RoutedEventArgs e)
    {
        CheckIfCategoryHasACustomOne();
        if (CheckIfRequiredFieldsAreFilled())
        {
            var todoId = Guid.NewGuid();
            var name = TodoNameTxtBox.Text;
            var dueDate = DueDatePicker.SelectedDate.Value;
            var dueTime = DueTimePicker.SelectedTime.Value;
            var repeatableType = (TodoRepeatableType)FrequencyComboBox.SelectedIndex;
            var todoStatus = TodoStatusType.None;
            var notes = new List<string> { NotesTxtBox.Text };
            var links = _linksAttached;
            var category = new List<TodoCategoryModel>();
            if (CategoryComboBox.SelectedIndex == 9)
            {
                _todoManager.AddCustomCategory(CustomCategoryTxtBox.Text);
                category = new List<TodoCategoryModel>
                {
                    new()
                    {
                        CategoryName = TodoCategorys.Custom,
                        CustomCategoryName = CustomCategoryTxtBox.Text,
                    }
                };
            }
            else
            {
                category = new List<TodoCategoryModel>
                {
                    new()
                    {
                        CategoryName = (TodoCategorys)CategoryComboBox.SelectedIndex,
                    }
                };
            }

            _todoManager.AddTodo(todoId, name, dueDate, dueTime, repeatableType, todoStatus, _filesAttached, notes,
                links, category);
        }
    }

    private bool CheckIfRequiredFieldsAreFilled()
    {
        if (!string.IsNullOrWhiteSpace(TodoNameTxtBox.Text) && DueDatePicker.SelectedDate != null &&
            DueTimePicker.SelectedTime != null && FrequencyComboBox.SelectedIndex != -1 &&
            CategoryComboBox.SelectedIndex != -1) return true;
        var customMessageBoxWindow = new CubeMessageBox
        {
            TitleText = { Text = "Error" },
            MessageText = { Text = "Please fill in all required fields" }
        };

        customMessageBoxWindow.ShowDialog();
        MakeRequiredFieldsRed();
        return false;
    }

    private void MakeRequiredFieldsRed()
    {
        if (string.IsNullOrWhiteSpace(TodoNameTxtBox.Text))
            TodoNameTxtBox.BorderBrush = new SolidColorBrush(Colors.Red);
        if (DueDatePicker.SelectedDate == null)
            DueDatePicker.BorderBrush = new SolidColorBrush(Colors.Red);
        if (DueTimePicker.SelectedTime == null)
            DueTimePicker.BorderBrush = new SolidColorBrush(Colors.Red);
        if (FrequencyComboBox.SelectedIndex == -1)
            FrequencyComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
        if (CategoryComboBox.SelectedIndex == -1)
            CategoryComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
    }

    private bool CheckIfCategoryHasACustomOne()
    {
        if (CategoryComboBox.SelectedIndex != 9 || !string.IsNullOrWhiteSpace(CustomCategoryTxtBox.Text)) return false;
        _todoManager.AddCustomCategory(CustomCategoryTxtBox.Text);
        return true;
    }
}