using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CubeManager.Helpers;
using CubeManager.Todos.Models;
using MaterialDesignThemes.Wpf;

namespace CubeManager.Todos;

public partial class TodoControl : UserControl
{
    private static readonly TodoData TodoData = new();

    public TodoControl()
    {
        InitializeComponent();
    }

    private void TodoControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        SetDefaultValues();
        GetValuesFromConfig();
        UpdateAny();
    }

    private void GetValuesFromConfig()
    {
        TodoData.Folders = ConfigManager.Instance.Config.Todo.Folders;
        TodoData.Settings = ConfigManager.Instance.Config.Todo.Settings;
        TodoData.Todos = ConfigManager.Instance.Config.Todo.Todos;
    }

    private void SetSelectedFolder()
    {
        foreach (var folder in TodoData.Folders.Where(folder => folder.Id == TodoData.Settings[0].SelectedFolder))
        {
            FolderList.SelectedItem = folder;
            FolderComboBox.SelectedItem = folder;
        }
    }

    private void UpdateConfig()
    {
        ConfigManager.Instance.UpdateConfig(config => config.Todo = TodoData);
    }

    private void UpdateAny()
    {
        SetFolderItems();
        UpdateStatusBar();
        SetSelectedFolder();
        UpdateConfig();
    }

    private void SetFolderItems()
    {
        FolderComboBox.Items.Clear();
        FolderList.Items.Clear();
        foreach (var folder in TodoData.Folders)
        {
            FolderList.Items.Add(folder);
            FolderComboBox.Items.Add(folder);
        }
    }

    private void UpdateStatusBar()
    {
        StatusBar.Text =
            $"Total Folders: {TodoData.Settings[0].TotalFolders} |" +
            $" Total Todos: {TodoData.Settings[0].TotalTodos} |" +
            $" Total Important Todos: {TodoData.Settings[0].TotalImportantTodos} |" +
            $" Total Planned Todos: {TodoData.Settings[0].TotalPlannedTodos} |" +
            $" Total Completed Todos: {TodoData.Settings[0].TotalCompletedTodos}";
        if (TodoData.Settings[0].TotalFolders != 3)
            StatusBar.Text += $" | Total Custom Folders: {TodoData.Settings[0].TotalFolders - 3}";
    }

    private void SetDefaultValues()
    {
        if (TodoData.Settings.Count == 0)
            TodoData.Settings.Add(new TodoSettings
            {
                TotalFolders = 0,
                TotalTodos = 0,
                TotalImportantTodos = 0,
                TotalPlannedTodos = 0,
                TotalCompletedTodos = 0
            });

        if (TodoData.Folders.Count != 0) return;
        AddDefaultFolders();
        TodoData.Settings[0].TotalFolders = 3;
        TodoData.Settings[0].SelectedFolder = TodoData.Folders[0].Id;
    }

    private void AddDefaultFolders()
    {
        TodoData.Folders.Add(new FolderItem
        {
            Name = "All",
            IconKind = PackIconKind.ToDo,
            Id = Guid.NewGuid()
        });

        TodoData.Folders.Add(new FolderItem
        {
            Name = "Important",
            IconKind = PackIconKind.Star,
            Id = Guid.NewGuid()
        });

        TodoData.Folders.Add(new FolderItem
        {
            Name = "Planned",
            IconKind = PackIconKind.Clock,
            Id = Guid.NewGuid()
        });
    }

    private void FolderList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FolderList.SelectedItem != null)
            TodoData.Settings[0].SelectedFolder = ((FolderItem)FolderList.SelectedItem).Id;
        UpdateConfig();
    }


    private void DialogHostOperation_OnDialogOpened(object sender, DialogOpenedEventArgs eventargs)
    {
        TodoGrid.IsEnabled = false;
    }

    private void DialogHostOperation_OnDialogClosing(object sender, DialogClosingEventArgs eventargs)
    {
        TodoGrid.IsEnabled = true;
    }

    private void AddFolderBtn_OnClick(object sender, RoutedEventArgs e)
    {
        AddFolderGrid.Visibility = Visibility.Visible;
        DialogHostOperation.IsOpen = true;
    }

    private void ChangeFolderIconDialogBtn_OnClick(object sender, RoutedEventArgs e)
    {
        IconDialogListBox.Visibility = Visibility.Visible;
        IconDialogListBox.SelectionMode = SelectionMode.Single;
        IconDialogSearchTextBox.Visibility = Visibility.Visible;
        if (IconDialogListBox.Items.Count == 0)
            FetchAllIconsAndSetDialogIcons();
    }

    private void IconDialogListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IconDialogListBox.SelectedItems.Count == 0) return;
        var packIcon = (PackIcon)IconDialogListBox.SelectedItem;
        ChangeFolderIconDialogBtn.Content = packIcon;
        IconDialogListBox.Visibility = Visibility.Collapsed;
        IconDialogSearchTextBox.Visibility = Visibility.Collapsed;
    }

    private void FetchAllIconsAndSetDialogIcons()
    {
        StatusBarDialogTextBlock.Visibility = Visibility.Visible;

        var thread = new Thread(() =>
        {
            foreach (var icon in Enum.GetValues(typeof(PackIconKind)))
                Dispatcher.Invoke(() =>
                {
                    StatusBarDialogTextBlock.Text = $"Loading {icon}...";
                    IconDialogListBox.IsEnabled = false;
                    var packIcon = new PackIcon { Kind = (PackIconKind)icon };
                    IconDialogListBox.Items.Add(packIcon);
                });
            Dispatcher.Invoke(() =>
            {
                StatusBarDialogTextBlock.Visibility = Visibility.Collapsed;
                IconDialogSearchTextBox.Visibility = Visibility.Visible;
                IconDialogListBox.IsEnabled = true;
            });
        });
        thread.Start();
    }

    private void IconDialogSearchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (IconDialogSearchTextBox.Text == string.Empty)
        {
            IconDialogListBox.Items.Clear();
            FetchAllIconsAndSetDialogIcons();
            return;
        }

        IconDialogListBox.Items.Clear();
        foreach (var icon in Enum.GetValues(typeof(PackIconKind)))
        {
            var packIcon = new PackIcon { Kind = (PackIconKind)icon };
            if (packIcon.Kind.ToString().ToLower().Contains(IconDialogSearchTextBox.Text.ToLower()))
                IconDialogListBox.Items.Add(packIcon);
        }
    }

    private void AddFolderDialogBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(AddFolderDialogBtnTextBlock.Text)) return;

        TodoData.Folders.Add(new FolderItem
        {
            Name = AddFolderDialogBtnTextBlock.Text,
            IconKind = ((PackIcon)ChangeFolderIconDialogBtn.Content).Kind,
            Id = Guid.NewGuid()
        });
        TodoData.Settings[0].TotalFolders++;
        UpdateAny();
        DialogHostOperation.IsOpen = false;
        AddFolderGrid.Visibility = Visibility.Collapsed;
    }

    private void AddFolderDialogBtnTextBlock_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        AddFolderDialogBtn_OnClick(sender, e);
        AddFolderGrid.Visibility = Visibility.Collapsed;
    }

    private void AddTodoBtn_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void AddTodoTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            AddTodoBtn_OnClick(sender, e);
    }
}