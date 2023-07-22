using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CubeManager.Helpers;
using CubeManager.Models;
using MaterialDesignThemes.Wpf;

namespace CubeManager.Controls;

public partial class TodosControl : UserControl
{
    private readonly Brush _darkBackground;
    private readonly Brush _darkSeparatorBackground;
    private readonly Logger _logger;
    private readonly SoundManager _soundManager = new();
    private bool _isEditMode;
    private Guid _selectedTodoId;

    public TodosControl()
    {
        _logger = new Logger();
        _darkSeparatorBackground = (Brush)Application.Current.Resources["MaterialDesignDarkSeparatorBackground"];
        _darkBackground = (Brush)Application.Current.Resources["MaterialDesignDarkBackground"];
        InitializeComponent();
    }

    private List<TodoItem> Todos => ConfigManager.Instance.Config.Todo.Todos;

    private void TodosControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        _isEditMode = false;
        foreach (var todo in ConfigManager.Instance.Config.Todo.Todos)
        {
            var card = CreateCard(todo);
            ConfigureCardEvents(todo.Id, card);
            TodoList.Children.Add(card);
        }

        TodoNewCalendar.SelectedDate = DateTime.Now;
        TodoNewTimePicker.SelectedTime = DateTime.Now;
        _logger.PrioInfo("TodosControl loaded successfully");
        TryClearSnackbarQueue();
    }

    private void TryClearSnackbarQueue()
    {
        InfoSnackbar.MessageQueue?.Clear();
    }

    private void AddTodoBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleTodoInputVisibility();
    }

    private void ToggleTodoInputVisibility()
    {
        TodoNewCard.Visibility = Visibility.Visible;
        TodoNewInput.Focus();
        AddTodoBtn.Visibility = Visibility.Collapsed;
        TodoNewTimePicker.Visibility = Visibility.Collapsed;
        TodoNewCalendar.Visibility = Visibility.Collapsed;
    }

    private void ToggleTodoButtonVisibility()
    {
        TodoNewCard.Visibility = Visibility.Collapsed;
        AddTodoBtn.Visibility = Visibility.Visible;
        TodoNewTimePicker.Visibility = Visibility.Collapsed;
        TodoNewCalendar.Visibility = Visibility.Collapsed;
    }

    private void TodoNewDateBtn_OnClick(object sender, RoutedEventArgs e)
    {
        TodoNewCalendar.Visibility = Visibility.Visible;
        TodoNewTimePicker.Visibility = Visibility.Collapsed;
    }

    private void TodoNewTimeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        TodoNewTimePicker.Visibility = Visibility.Visible;
        TodoNewCalendar.Visibility = Visibility.Collapsed;
    }

    private void TodoNewAddBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (_isEditMode)
            UpdateTodo(_selectedTodoId);
        else
            AddNewTodo();
    }

    private void UpdateTodo(Guid toEditTodoId)
    {
        var todo = CreateTodoFromInputFields();
        if (!ValidationCheck(todo)) return;

        var oldCard = TodoList.Children.OfType<Card>().FirstOrDefault(card => (Guid)card.Tag == toEditTodoId);
        if (oldCard == null) return;
        TodoList.Children.Remove(oldCard);

        AddTodoCard(todo);

        var oldTodo = Todos.FirstOrDefault(t => t.Id == toEditTodoId);
        if (oldTodo == null) return;
        Todos.Remove(oldTodo);
        UpdateConfig();

        ResetInputFields();
        ToggleTodoButtonVisibility();
        TodoNewAddBtn.Content = "Add";
        _selectedTodoId = Guid.Empty;
        _isEditMode = false;
    }

    private void AddNewTodo()
    {
        _logger.PrioInfo("Adding new todo started");
        var todo = CreateTodoFromInputFields();
        if (!ValidationCheck(todo)) return;

        AddTodoCard(todo);
        ResetInputFields();
        ToggleTodoButtonVisibility();
        _logger.PrioInfo("New todo added successfully.");
    }

    private TodoItem CreateTodoFromInputFields()
    {
        return new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = TodoNewInput.Text,
            Description = TodoNewDescription.Text,
            DueDate = TodoNewCalendar.SelectedDate.GetValueOrDefault(),
            DueTime = TodoNewTimePicker.SelectedTime.GetValueOrDefault()
        };
    }

    private bool ValidationCheck(TodoItem todo)
    {
        string warningMessage = null;

        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            warningMessage = "Title cannot be empty";
        }
        else if (todo.DueDate == default)
        {
            warningMessage = "Date cannot be empty";
        }
        else if (todo.DueTime == default)
        {
            warningMessage = "Time cannot be empty";
        }

        if (warningMessage == null) return true;

        TryClearSnackbarQueue();
        EnqueueSnackbar(warningMessage);
        _logger.Warn("Todo validation failed: " + warningMessage);
        return false;
    }

    private void ResetInputFields()
    {
        TodoNewInput.Text = "";
        TodoNewDescription.Text = "";
    }

    private void TodoNewCalendar_OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        TryClearSnackbarQueue();
        EnqueueSnackbar("Date Selected: " + TodoNewCalendar.SelectedDate.GetValueOrDefault().ToShortDateString());
    }

    private void TodoNewTimePicker_OnSelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        TryClearSnackbarQueue();
        EnqueueSnackbar("Time Selected: " + TodoNewTimePicker.SelectedTime.GetValueOrDefault().ToShortTimeString());
    }

    private void EnqueueSnackbar(string message)
    {
        InfoSnackbar.MessageQueue?.Enqueue(message);
    }


    private void AddTodoCard(TodoItem todo)
    {
        var card = CreateCard(todo);
        ConfigureCardEvents(todo.Id, card);
        SaveCardToConfig(todo);
        TodoList.Children.Add(card);
    }

    private void ConfigureCardEvents(Guid id, Card card)
    {
        card.MouseDown += (sender, args) => OnCardMouseDown(id, card);
        card.MouseEnter += (sender, args) => card.Background = _darkBackground;
        card.MouseLeave += (sender, args) => card.Background = _darkSeparatorBackground;
    }

    private void OnCardMouseDown(Guid id, Card card)
    {
        _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CreditsGet);
        LevelUp();
        RemoveCard(id, card);
    }

    private void LevelUp()
    {
        (Window.GetWindow(this) as CubeMangerWindow)?.DoLevelUp();
    }

    private void RemoveCard(Guid id, Card card)
    {
        var todoToRemove = Todos.FirstOrDefault(todo => todo.Id == id);
        if (todoToRemove != null) Todos.Remove(todoToRemove);

        UpdateConfig();

        TodoList.Children.Remove(card);
    }


    private void SaveCardToConfig(TodoItem todo)
    {
        Todos.Add(todo);
        UpdateConfig();
        _logger.PrioInfo($"Todo saved with id: {todo.Id}");
    }

    private void UpdateConfig()
    {
        ConfigManager.Instance.UpdateConfig(config => config.Todo.Todos = Todos);
        _logger.PrioInfo("Todo config updated");
    }

    private Card CreateCard(TodoItem todo)
    {
        return new Card
        {
            Background = _darkSeparatorBackground,
            Content = CreateContent(todo),
            // ReSharper disable once HeapView.BoxingAllocation
            Tag = todo.Id
        };
    }

    private StackPanel CreateMainStackPanel(TodoItem todo)
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Children =
            {
                CreateUpperStackPanel(todo.Title, todo.Description),
                CreateLowerCard(todo)
            }
        };
    }

    private StackPanel CreateUpperStackPanel(string title, string description)
    {
        // Upper Stack Panel
        var upperStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new TextBlock
                {
                    Text = title, FontFamily = new FontFamily("Medium"), Margin = new Thickness(5),
                    Style = (Style)Application.Current.Resources["MaterialDesignHeadline4TextBlock"]
                },
                new TextBlock
                {
                    Text = description, Margin = new Thickness(15, 0, 5, 10),
                    FontFamily = new FontFamily("normal"),
                    Style = (Style)Application.Current.Resources["MaterialDesignCaptionTextBlock"]
                }
            }
        };

        return upperStackPanel;
    }

    private Card CreateLowerCard(TodoItem todo)
    {
        return new Card
        {
            Background = _darkSeparatorBackground,
            Content = CreateLowerStackPanel(todo)
        };
    }

    private UIElement CreateContent(TodoItem todo)
    {
        return new Border
        {
            BorderBrush = _darkSeparatorBackground,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            CornerRadius = new CornerRadius(5),
            BorderThickness = new Thickness(1),
            Child = CreateMainStackPanel(todo)
        };
    }

    private StackPanel CreateLowerStackPanel(TodoItem todo)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left,
            Children =
            {
                CreateIcon(PackIconKind.Calendar),
                CreateTextBlock(todo.DueDate.ToShortDateString()),
                CreateIcon(PackIconKind.Alarm),
                CreateTextBlock(todo.DueTime.ToShortTimeString()),
                CreateButton(PackIconKind.Edit, todo.Id),
                CreateButton(PackIconKind.Delete, todo.Id),
                CreateTransparentIdTextBlock(todo.Id)
            }
        };
    }

    private Button CreateButton(PackIconKind kind, Guid id)
    {
        var button = new Button
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            Style = (Style)Application.Current.Resources["MaterialDesignIconButton"],
            Foreground = (Brush)Application.Current.Resources["MaterialDesignDarkForeground"],
            Content = CreateIcon(kind)
        };

        button.Click += (sender, args) =>
        {
            switch (kind)
            {
                case PackIconKind.Edit:
                    var todoToEdit = Todos.FirstOrDefault(todo => todo.Id == id);
                    if (todoToEdit == null)
                    {
                        EnqueueSnackbar("Todo not found");
                        return;
                    }

                    _selectedTodoId = id;
                    _isEditMode = true;
                    ChangetoEditDesign(todoToEdit);
                    ToggleTodoInputVisibility();
                    break;
                case PackIconKind.Delete:
                    LevelUp();
                    foreach (var item in TodoList.Children.Cast<UIElement>().ToArray())
                    {
                        if (!(item is Card card)) continue;
                        var todo = FindAssociatedTodoWithCard(card, id);
                        if (todo != null)
                        {
                            Todos.Remove(todo);
                            TodoList.Children.Remove(card);
                            UpdateConfig();
                        }
                    }

                    _soundManager.PlayAudio(ConfigManager.Instance.Config.SoundSettings.CreditsGet);
                    break;
            }
        };
        return button;
    }

    private void ChangetoEditDesign(TodoItem todoToEdit)
    {
        TodoNewInput.Text = todoToEdit.Title;
        TodoNewDescription.Text = todoToEdit.Description;
        TodoNewCalendar.SelectedDate = todoToEdit.DueDate;
        TodoNewTimePicker.SelectedTime = todoToEdit.DueTime;
        TodoNewAddBtn.Content = "Save";
    }

    private TodoItem FindAssociatedTodoWithCard(Card card, Guid id)
    {
        var idBlock = FindTextBlock(card, id.ToString());

        if (idBlock == null)
            return null; //ignore since its getting deleted anyway

        Guid todoId;
        if (!Guid.TryParse(idBlock.Text, out todoId)) return null;

        return Todos.FirstOrDefault(todo => todo.Id == todoId);
    }

    private TextBlock CreateTextBlock(string text)
    {
        return new TextBlock
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5, 0, 0, 0)
        };
    }

    private PackIcon CreateIcon(PackIconKind kind)
    {
        return new PackIcon
        {
            Kind = kind,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5)
        };
    }

    private TextBlock CreateTransparentIdTextBlock(Guid id)
    {
        return new TextBlock
        {
            Text = id.ToString(),
            VerticalAlignment = VerticalAlignment.Center,
            Opacity = 0.2,
            Margin = new Thickness(5, 0, 0, 0)
        };
    }

    private TextBlock FindTextBlock(Visual parent, string id)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = (Visual)VisualTreeHelper.GetChild(parent, i);

            if (child is TextBlock block && block.Text == id) return block;

            var result = FindTextBlock(child, id);
            if (result != null) return result;
        }

        return null;
    }
}