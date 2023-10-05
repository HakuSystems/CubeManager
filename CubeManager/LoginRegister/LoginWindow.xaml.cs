using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CubeManager.ZenQuotes;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Wpf.Ui.Controls;

namespace CubeManager.LoginRegister;

public partial class LoginWindow : UiWindow
{
    private float _rotationAngle;
    private SKPoint _mousePosition;
    private SKPoint _lastMousePosition;
    private readonly DispatcherTimer _timer;
    private SKShader _sweepShader;

    public LoginWindow()
    {
        InitializeComponent();
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // 60 FPS
        };
        _timer.Tick += (s, e) => OnRendering();
        _timer.Start();
    }

    private void OnRendering()
    {
        _rotationAngle += 0.1f;
        if (_rotationAngle > 360)
            _rotationAngle -= 360;
        CanvasView.InvalidateVisual();
    }


    private void LoginWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void DisplayQuote()
    {
        QuoteTextBlock.Text = new FetchQuote().RetrieveQuote();
    }

    private void CanvasView_OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var info = e.Info;
        var surface = e.Surface;
        var canvas = surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        var center = new SKPoint(info.Width / 1, info.Height / 1);
        var colors = new[] { SKColor.Parse("#292929"), SKColor.Parse("#181818") };
        var colorPos = new[] { 0.0f, 1.0f };

        if (_sweepShader == null) _sweepShader = SKShader.CreateSweepGradient(center, colors, colorPos);

        var shaderRotationMatrix = SKMatrix.CreateRotationDegrees(_rotationAngle, center.X, center.Y);
        var rotatedShader = _sweepShader.WithLocalMatrix(shaderRotationMatrix);

        using (var paint = new SKPaint { Shader = rotatedShader })
        {
            canvas.DrawRect(new SKRect(0, 0, info.Width, info.Height), paint);
        }
    }

    private void LoginWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        DisplayQuote();
        PageContent.Source = new Uri("LoginContent.xaml", UriKind.Relative);
    }

    private void CanvasMouseView_OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        using (var paint = new SKPaint())
        {
            paint.Color = SKColors.LightBlue.WithAlpha(128); // Semi-transparent light blue
            paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 50); // Blur effect

            var glowRect = new SKRect(_mousePosition.X - 50, _mousePosition.Y - 50, _mousePosition.X + 50,
                _mousePosition.Y + 50);
            canvas.DrawRect(glowRect, paint);
        }
    }

    private void LoginWindow_OnMouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(CanvasMouseView);
        _mousePosition = new SKPoint((float)position.X, (float)position.Y);

        if (SKPoint.Distance(_mousePosition, _lastMousePosition) > 5)
        {
            CanvasMouseView.InvalidateVisual();
            _lastMousePosition = _mousePosition;
        }
    }
}