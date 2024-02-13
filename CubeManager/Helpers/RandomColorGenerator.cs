using System.Drawing;

namespace CubeManager.Helpers;

public class RandomColorGenerator
{
    public static Color GenerateColorWithAlpha(int alpha)
    {
        var random = new Random();
        return Color.FromArgb(alpha, random.Next(256), random.Next(256), random.Next(256));
    }
}