using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace CubeManager.LoginRegister;

public partial class RegisterContent : UiPage
{
    public RegisterContent()
    {
        InitializeComponent();
    }

    private void LoginBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var loginWindow = (LoginWindow) Window.GetWindow(this);
        loginWindow.PageContent.Source = new Uri("LoginContent.xaml", UriKind.Relative);
    }

    private void RegisterBtn_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}