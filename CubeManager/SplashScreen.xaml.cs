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
        DisplayQuote();
        StartProgressTimer();
    }

    private void DisplayQuote()
    {
        QuoteText.Text = new FetchQuote().RetrieveQuote();
    }

    private void StartProgressTimer()
    {
        double progress = 0;
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        timer.Tick += (sender, args) => OpenNewWindowAfterProgress(timer, ref progress);
        timer.Start();
    }

    private void OpenNewWindowAfterProgress(DispatcherTimer timer, ref double progress)
    {
        progress += 0.01f;
        if (progress >= 1.0)
        {
            timer.Stop();
            var mainWindow = new CubeMangerWindow();
            mainWindow.Show();
            Close();
        }
    }
}