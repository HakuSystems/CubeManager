using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        //DialogTimePicker is 24h only if the windows time format is 24h
        DialogTimePicker.Is24Hours = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Contains("H");

        HideDialogHostContents();
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
        if (FolderList.SelectedItem == null) return;
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
        HideDialogHostContents();
    }

    private void HideDialogHostContents()
    {
        DialogSelectDateAndTimeText.Visibility = Visibility.Collapsed;
        DialogTodoNewCalendar.Visibility = Visibility.Collapsed;
        AddFolderGrid.Visibility = Visibility.Collapsed;
        DialogTimePicker.Visibility = Visibility.Collapsed;
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

    private void AddTodoDateBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var contextMenu = new ContextMenu();
        contextMenu.Items.Add(new MenuItem { Header = $"Today | {DateTime.Today.DayOfWeek}" });
        contextMenu.Items.Add(new MenuItem { Header = $"Tomorrow | {DateTime.Today.AddDays(1).DayOfWeek}" });
        contextMenu.Items.Add(new MenuItem { Header = "Next Week | Monday" }); // Always Monday
        contextMenu.Items.Add(new MenuItem { Header = "Select Date" });
        if (AddTodoDateBtn.Content is StackPanel)
            contextMenu.Items.Add(new MenuItem { Header = "Remove Date", Foreground = Brushes.Red });


        contextMenu.PlacementTarget = AddTodoDateBtn;
        contextMenu.IsOpen = true;

        contextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler((o, args) =>
        {
            switch (((MenuItem)args.OriginalSource).Header.ToString())
            {
                case var s when s.StartsWith("Today"):
                    AddTodoDateBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.CalendarToday,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = DateTime.Today.DayOfWeek.ToString(),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case var s when s.StartsWith("Tomorrow"):
                    AddTodoDateBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.CalendarPlus,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = DateTime.Today.AddDays(1).DayOfWeek.ToString(),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case "Next Week | Monday":
                    var nextMonday = DateTime.Today.AddDays(1);
                    while (nextMonday.DayOfWeek != DayOfWeek.Monday)
                        nextMonday = nextMonday.AddDays(1);
                    AddTodoDateBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.CalendarWeek,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = nextMonday.ToShortDateString(),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case "Select Date":
                    DialogHostOperation.IsOpen = true;
                    TimeOrDatePickerGrid.Visibility = Visibility.Visible;
                    DialogTodoNewCalendar.Visibility = Visibility.Visible;
                    DialogSelectDateAndTimeText.Visibility = Visibility.Visible;
                    DialogTodoNewCalendar.SelectedDate = DateTime.Today;
                    DialogSelectDateAndTimeText.Text = "Select Date";
                    ChangeToSelectedDate();
                    break;
                case "Remove Date":
                    // Remove the date
                    AddTodoDateBtn.Content = new PackIcon
                    {
                        Kind = PackIconKind.Calendar,
                        Margin = new Thickness(5),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    break;
                default:
                    throw new InvalidOperationException("Unrecognized menu item");
            }
        }));
    }

    private void DialogTodoNewCalendar_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        ChangeToSelectedDate();
    }

    private void ChangeToSelectedDate()
    {
        AddTodoDateBtn.Content = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(5),
            Children =
            {
                new PackIcon
                {
                    Kind = PackIconKind.Calendar,
                    Margin = new Thickness(0, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
                new TextBlock
                {
                    Text = DialogTodoNewCalendar.SelectedDate.GetValueOrDefault().ToShortDateString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            }
        };
    }

    private void AddTodoTimeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var contextMenu = new ContextMenu();
        contextMenu.Items.Add(new MenuItem { Header = "Later that day" });
        contextMenu.Items.Add(new MenuItem { Header = "Next Hour" });
        contextMenu.Items.Add(new MenuItem { Header = "Custom time" });
        if (AddTodoTimeBtn.Content is StackPanel)
            contextMenu.Items.Add(new MenuItem { Header = "Remove Time", Foreground = Brushes.Red });

        contextMenu.PlacementTarget = AddTodoDateBtn;
        contextMenu.IsOpen = true;

        contextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler((o, args) =>
        {
            switch (((MenuItem)args.OriginalSource).Header.ToString())
            {
                case "Later that day":
                    AddTodoTimeBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.Clock,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = $"+3 Hours | {DateTime.Now.AddHours(3).ToShortTimeString()}",
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case "Next Hour":
                    AddTodoTimeBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.Clock,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = $"Next Hour | {DateTime.Now.AddHours(1).ToShortTimeString()}",
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case "Custom time":
                    DialogHostOperation.IsOpen = true;
                    TimeOrDatePickerGrid.Visibility = Visibility.Visible;
                    DialogSelectDateAndTimeText.Text = "Select time";
                    DialogSelectDateAndTimeText.Visibility = Visibility.Visible;
                    DialogTimePicker.Visibility = Visibility.Visible;
                    DialogTimePicker.SelectedTime = DateTime.Today;
                    ChangeToSelectedTime();
                    break;
                case "Remove Time":
                    AddTodoTimeBtn.Content = new PackIcon
                    {
                        Kind = PackIconKind.Clock,
                        Margin = new Thickness(5),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    break;
                default:
                    throw new InvalidOperationException("Unrecognized menu item");
            }
        }));
    }

    private void ChangeToSelectedTime()
    {
        AddTodoTimeBtn.Content = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(5),
            Children =
            {
                new PackIcon
                {
                    Kind = PackIconKind.Clock,
                    Margin = new Thickness(0, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
                new TextBlock
                {
                    Text = DialogTimePicker.SelectedTime.GetValueOrDefault().ToShortTimeString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            }
        };
    }

    private void DialogTimePicker_OnSelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        ChangeToSelectedTime();
    }

    private void RenewTodoBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var contextMenu = new ContextMenu();
        contextMenu.Items.Add(new MenuItem { Header = "Daily" });
        contextMenu.Items.Add(new MenuItem { Header = "Weekly" });
        contextMenu.Items.Add(new MenuItem { Header = "Monthly" });
        contextMenu.Items.Add(new MenuItem { Header = "Yearly" });
        if (RenewTodoBtn.Content is StackPanel)
            contextMenu.Items.Add(new MenuItem { Header = "Dont Renew", Foreground = Brushes.Red });

        contextMenu.PlacementTarget = RenewTodoBtn;
        contextMenu.IsOpen = true;

        contextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler((o, args) =>
        {
            switch (((MenuItem)args.OriginalSource).Header.ToString())
            {
                case "Daily":
                    RenewTodoBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.Autorenew,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = "Daily",
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case "Weekly":
                    RenewTodoBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.Autorenew,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = "Weekly",
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case "Monthly":
                    RenewTodoBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.Autorenew,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = "Monthly",
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case "Yearly":
                    RenewTodoBtn.Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new PackIcon
                            {
                                Kind = PackIconKind.Autorenew,
                                Margin = new Thickness(0, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            new TextBlock
                            {
                                Text = "Yearly",
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };
                    break;
                case "Dont Renew":
                    RenewTodoBtn.Content = new PackIcon
                    {
                        Kind = PackIconKind.Autorenew,
                        Margin = new Thickness(0, 0, 5, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    break;
                default:
                    throw new InvalidOperationException("Unrecognized menu item");
            }
        }));
    }
}