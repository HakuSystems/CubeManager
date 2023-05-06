using System.Windows;
using System.Windows.Input;

namespace CubeManager;

public partial class CubeMangerWindow : Window
{
    public CubeMangerWindow()
    {
        InitializeComponent();
    }

    private void DragGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void MinBtn_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void ClosBtn_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}