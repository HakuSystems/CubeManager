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
            TotalContent = TodoData.Settings[0].TotalTodos.ToString(),
            IconKind = PackIconKind.ToDo,
            Id = Guid.NewGuid(),
            isDefault = true
        });

        TodoData.Folders.Add(new FolderItem
        {
            Name = "Important",
            TotalContent = TodoData.Settings[0].TotalImportantTodos.ToString(),
            IconKind = PackIconKind.Star,
            Id = Guid.NewGuid(),
            isDefault = true
        });

        TodoData.Folders.Add(new FolderItem
        {
            Name = "Planned",
            TotalContent = TodoData.Settings[0].TotalPlannedTodos.ToString(),
            IconKind = PackIconKind.Clock,
            Id = Guid.NewGuid(),
            isDefault = true
        });
    }

    private void FolderList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FolderList.SelectedItem == null) return;
        TodoData.Settings[0].SelectedFolder = ((FolderItem)FolderList.SelectedItem).Id;

        if (Mouse.RightButton != MouseButtonState.Pressed) return;
        if (FolderList.SelectedItem is FolderItem folderItem && folderItem.isDefault)
            return;

        var newTread = new Thread(() =>
        {
            Dispatcher.Invoke(() =>
            {
                var contextMenu = new ContextMenu();
                contextMenu.Items.Add(new MenuItem { Header = "Delete Folder", Foreground = Brushes.Red });

                contextMenu.PlacementTarget = FolderList;
                contextMenu.IsOpen = true;

                contextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler((o, args) =>
                {
                    switch (((MenuItem)args.OriginalSource).Header.ToString())
                    {
                        case "Delete Folder":
                            TodoData.Folders.Remove((FolderItem)FolderList.SelectedItem);
                            FolderList.Items.Remove(FolderList.SelectedItem);
                            UpdateAny();
                            break;
                        default:
                            throw new InvalidOperationException("Unrecognized menu item");
                    }
                }));
            });
        });
        newTread.Start();
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
            TotalContent = "0",
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
        var dueDate = DialogTodoNewCalendar.SelectedDate;
        var dueTime = DialogTimePicker.SelectedTime;
        string? repeat = null;
        FolderItem? inFolder = null;
        
        if (RenewTodoBtn.Content is StackPanel)
            repeat = ((TextBlock)((StackPanel)RenewTodoBtn.Content).Children[1]).Text;
        if (FolderComboBox.SelectedItem is FolderItem)
            inFolder = (FolderItem)FolderComboBox.SelectedItem;

        if (string.IsNullOrEmpty(AddTodoTextBox.Text)) return;
        ActiveListContent.Items.Add(CreateCardUi(AddTodoTextBox.Text, null, dueDate, dueTime, repeat, false, false,
            inFolder));
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
            RenewTodoBtn.Content = ((MenuItem)args.OriginalSource).Header.ToString() switch
            {
                "Daily" => new StackPanel
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
                },
                "Weekly" => new StackPanel
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
                },
                "Monthly" => new StackPanel
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
                },
                "Yearly" => new StackPanel
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
                },
                "Dont Renew" => new PackIcon
                {
                    Kind = PackIconKind.Autorenew,
                    Margin = new Thickness(0, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
                _ => throw new InvalidOperationException("Unrecognized menu item")
            };
        }));
    }

    private void FolderComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FolderComboBox.SelectedItem is not FolderItem)
            FolderList.SelectedItem = FolderList.Items[0];
    }


    private Card CreateCardUi(string Title, string Description = null, DateTime? DueDate = null,
        DateTime? DueTime = null, string? Repeat = null, bool MarkedAsImportant = false,
        bool MarkedAsCompleted = false, FolderItem? InFolder = null, string Notes = null, List<LinkItem> Links = null)
    {
        // Main Card
        var mainCard = new Card();

        var mainStackPanel = new StackPanel();
        mainCard.Content = mainStackPanel;

        // Top Buttons StackPanel
        var topButtonsStackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        mainStackPanel.Children.Add(topButtonsStackPanel);

        // Done Button Card
        var doneButtonCard = new Card
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5)
        };
        topButtonsStackPanel.Children.Add(doneButtonCard);

        var doneButton = new Button
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(5),
            Content = new PackIcon { Kind = PackIconKind.Done, Opacity = 0.5, Foreground = Brushes.White },
            Style = (Style)FindResource("MaterialDesignIconButton")
        };
        doneButtonCard.Content = doneButton;

        // Star Button Card
        var starButtonCard = new Card
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5)
        };
        topButtonsStackPanel.Children.Add(starButtonCard);

        var starButton = new Button
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(5),
            Content = new PackIcon { Kind = PackIconKind.Star, Foreground = Brushes.White },
            Style = (Style)FindResource("MaterialDesignIconButton")
        };
        starButtonCard.Content = starButton;

        // Title TextBox
        var titleTextBox = new TextBox
        {
            Text = Title,
            FontSize = 20,
            Margin = new Thickness(15, 5, 5, 5)
        };
        mainStackPanel.Children.Add(titleTextBox);

// Description TextBox
        var descriptionTextBox = new TextBox
        {
            ToolTip = "Description",
            FontSize = 20,
            Margin = new Thickness(15, 5, 5, 5)
        };
        mainStackPanel.Children.Add(descriptionTextBox);

// Date and Time StackPanel
        var dateTimeStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
        mainStackPanel.Children.Add(dateTimeStackPanel);

// Due Date Card
        var dueDateCard = new Card { Margin = new Thickness(5) };
        dateTimeStackPanel.Children.Add(dueDateCard);

        var dueDateStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
        dueDateCard.Content = dueDateStackPanel;

        var calendarIcon = new PackIcon
        {
            Kind = PackIconKind.Calendar, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5),
            Foreground = Brushes.White
        };
        dueDateStackPanel.Children.Add(calendarIcon);

        var dueDateTextBlock = new TextBlock
        {
            Text = DueDate == null ? "Due Date: None" : $"Due Date: {DueDate.Value.ToShortDateString()}",
            FontSize = 20, Margin = new Thickness(5),
            VerticalAlignment = VerticalAlignment.Center
        };
        dueDateStackPanel.Children.Add(dueDateTextBlock);

// Due Time Card
        var dueTimeCard = new Card { Margin = new Thickness(5) };
        dateTimeStackPanel.Children.Add(dueTimeCard);

        var dueTimeStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
        dueTimeCard.Content = dueTimeStackPanel;

        var clockIcon = new PackIcon
        {
            Kind = PackIconKind.Clock, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5),
            Foreground = Brushes.White
        };
        dueTimeStackPanel.Children.Add(clockIcon);

        var dueTimeTextBlock = new TextBlock
        {
            Text = DueTime == null ? "Due Time: None" : $"Due Time: {DueTime.Value.ToShortTimeString()}",
            FontSize = 20, Margin = new Thickness(5),
            VerticalAlignment = VerticalAlignment.Center
        };
        dueTimeStackPanel.Children.Add(dueTimeTextBlock);

// Repeat Card
        var repeatCard = new Card { Margin = new Thickness(5) };
        dateTimeStackPanel.Children.Add(repeatCard);

        var repeatStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
        repeatCard.Content = repeatStackPanel;

        var repeatIcon = new PackIcon
        {
            Kind = PackIconKind.Repeat, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5),
            Foreground = Brushes.White
        };
        repeatStackPanel.Children.Add(repeatIcon);

        var repeatTextBlock = new TextBlock
        {
            Text = Repeat ?? "None",
            HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(5),
            VerticalAlignment = VerticalAlignment.Center
        };
        repeatStackPanel.Children.Add(repeatTextBlock);

// Notes TextBox
        var notesTextBox = new TextBox
        {
            ToolTip = "Add Notes",
            FontSize = 20,
            Margin = new Thickness(15, 5, 5, 5)
        };
        mainStackPanel.Children.Add(notesTextBox);

// Links Card
        var linksCard = new Card();
        mainStackPanel.Children.Add(linksCard);

        var linksStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
        linksCard.Content = linksStackPanel;

        var linkTextBox = new TextBox
        {
            Text = "https://www.google.com",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5)
        };
        linksStackPanel.Children.Add(linkTextBox);

        var addButton = new Button
        {
            Content = new PackIcon { Kind = PackIconKind.Plus },
            Foreground = Brushes.White,
            Style = (Style)FindResource("MaterialDesignIconButton")
        };
        linksStackPanel.Children.Add(addButton);

// ListView
        var listView = new ListView();
        mainStackPanel.Children.Add(listView);

// FolderName Card
        var folderNameCard = new Card
        {
            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5)
        };
        mainStackPanel.Children.Add(folderNameCard);

        var folderNameStackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal, Margin = new Thickness(5),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                new PackIcon
                {
                    Kind = InFolder?.IconKind ?? PackIconKind.Folder,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5),
                    Foreground = Brushes.White
                },
                new TextBlock
                {
                    Text = InFolder == null ? "None" : InFolder.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5),
                    FontSize = 20
                }
            }
        };
        folderNameCard.Content = folderNameStackPanel;

        return mainCard;
    }
}