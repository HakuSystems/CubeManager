using System.Windows;
using System.Windows.Threading;
using CubeManager.ZenQuotes;

namespace CubeManager;

public partial class SplashScreen : Window
{
    public SplashScreen()
    {
        InitializeComponent();
    }

    private void SplashScreen_OnLoaded(object sender, RoutedEventArgs e)
    {
        double progress = 0;
        QuoteText.Text = new FetchQuote().RetrieveQuote();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };

        timer.Tick += (sender, args) =>
        {
            progress += 0.01f;
            ProgressBar.Value = progress * 100;

            if (progress >= 1.0)
            {
                timer.Stop();
                var mainWindow = new CubeMangerWindow();
                mainWindow.Show();
                Close();
            }
        };

        timer.Start();
    }
}