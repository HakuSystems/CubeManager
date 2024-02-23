using System.Windows.Controls;
using System.Windows.Media;
using CubeManager.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace CubeManager.Controls.Subscriptions;

public partial class NewSubscriptionUI : UserControl
{
    private SKPoint _mousePosition;
    public NewSubscriptionUI()
    {
        InitializeComponent();
    }
}