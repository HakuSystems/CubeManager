using System.Windows;
using Wpf.Ui.Controls;

namespace CubeManager.CustomMessageBox;

public partial class CubeMessageBox : UiWindow
{
    public CubeMessageBox()
    {
        InitializeComponent();
        this.DataContext = DataContext;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}