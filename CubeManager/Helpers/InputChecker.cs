using System.Text.RegularExpressions;

namespace CubeManager.Helpers;

public class InputChecker
{
    public static bool ValidateEmail(string email)
    {
        var pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.(com|net|org|edu|gov|mil|biz|info|mobi|name|aero|jobs|museum|coop|asia|eu|us|ca)$";
        return Regex.IsMatch(email, pattern);
    }
    public static bool ValidatePrice(string price)
    {
        price = price.Replace(',', '.');
        return decimal.TryParse(price, out _);
    }
}