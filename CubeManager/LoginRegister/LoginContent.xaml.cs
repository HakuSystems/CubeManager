using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace CubeManager.LoginRegister;

public partial class LoginContent : UiPage
{
    public LoginContent()
    {
        InitializeComponent();
    }

    private void LoginBtn_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void RegisterBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var loginWindow = (LoginWindow) Window.GetWindow(this);
        loginWindow.PageContent.Source = new Uri("RegisterContent.xaml", UriKind.Relative);
    }
}